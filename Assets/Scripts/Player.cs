using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Player : Entity
{
    [SerializeField] private float MAX_SPEED;
    [SerializeField] private float FIRE_COOL_TIME;
    [SerializeField] private float JUMP_COOL_TIME;
    [SerializeField] private float JUMP_FORCE;
    [SerializeField] private AudioClip bulletSound;
    [SerializeField] private AudioClip playerDamageSound;
    [SerializeField] private AudioClip playerDeathSound;

    private Button returnButton;
    private FixedJoystick joystick;

    private float lastFiredTime = -1;
    private float lastJumpedTime = -1;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GameObject statusPanel = GameObject.Find("StatusPanel");
        
        foreach(Transform transform in statusPanel.transform)
        {
            Button button;
            if(transform.TryGetComponent(out button))
            {
                if (button.name == "ReturnButton") returnButton = button;
            }
            FixedJoystick jstick;
            if(transform.TryGetComponent(out jstick)){
                joystick = jstick;
            }
            
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Game game = Game.Get();

        joystick.enabled = !Game.isPausing;
        if (!Game.isPausing)
        {
            Control();
        }
        SetHP();

        // 背景移動
        float xDiffMax = 235;
        float yDiffMax = 240;
        float maxBackMoveX = 2 * xDiffMax / (game.stageMaxX - game.stageMinX - 2);
        float maxBackMoveY = 2 * yDiffMax / (game.stageMaxY - game.stageMinY - 2);
        float maxBackMove = Mathf.Min(maxBackMoveX, maxBackMoveY, 4);
        float centerX = (game.stageMinX + game.stageMaxX) / 2.0f;
        float centerY = (game.stageMinY + game.stageMaxY) / 2.0f;
        GameObject background = GameObject.Find("Background");
        background.transform.localPosition = new Vector3(
            maxBackMove * (centerX - transform.position.x),
            maxBackMove * (centerY - transform.position.y),
            0
        );
    }

    private void Control()
    {
        Game game = Game.Get();
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        // 操作
        Vector2 v = new Vector2(
            0.6f * Input.GetAxisRaw("Horizontal") + 0.4f * Input.GetAxis("Horizontal"),
            0.6f * Input.GetAxisRaw("Vertical") + 0.4f * Input.GetAxis("Vertical")
        );
        v.x += 2.5f * joystick.Horizontal;
        v.y += 2.5f * joystick.Vertical;

        // プレイヤーの向き
        List<float> velArr = new List<float> { v.y, -v.x, -v.y, v.x };
        float maxVel = velArr.Max();
        if (maxVel > 0)
        {
            float zAngle = velArr.IndexOf(maxVel) * 90;
            Vector3 ang = transform.eulerAngles;
            ang.z = zAngle;
            transform.eulerAngles = ang;
        }

        // プレイヤーのジャンプとキャンセル
        bool jump = false, jumpCancel = false;
        if (IsGround())
        {
            float dot = Vector2.Dot(game.gravity, v);
            jump = dot <= -0.6f &&
                game.time - lastJumpedTime >= JUMP_COOL_TIME;
        }
        if (game.gravity != Vector2.zero && !IsGround())
        {
            float dot = Vector2.Dot(game.gravity, v);
            // ジャンプキーを押し続けていない & 上昇中の場合
            if (
                    dot > -0.6f &&
                    Vector2.Dot(game.gravity, rigidbody.velocity) < 0
                )
            {
                // ジャンプキャンセル(上昇速度を減衰)
                jumpCancel = true;
            }
        }

        // プレイヤーの速度
        v.x = Mathf.Clamp(v.x, -1, 1);
        v.y = Mathf.Clamp(v.y, -1, 1);
        if (game.gravity == Vector2.right || game.gravity == Vector2.left)
        {
            v.x = rigidbody.velocity.x / MAX_SPEED;
        }
        if (game.gravity == Vector2.up || game.gravity == Vector2.down)
        {
            v.y = rigidbody.velocity.y / MAX_SPEED;
        }
        if (jump)
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.AddForce(-game.gravity * JUMP_FORCE);
            if (game.gravity == Vector2.right || game.gravity == Vector2.left)
            {
                v.x = 0;
            }
            else
            {
                v.y = 0;
            }
            lastJumpedTime = game.time;
        }
        if (jumpCancel)
        {
            float coeff = 2.0f;
            if (game.gravity == Vector2.right)
            {
                v.x = Mathf.Max(v.x - coeff * Time.deltaTime, 0);
            }
            else if (game.gravity == Vector2.left)
            {
                v.x = Mathf.Min(v.x + coeff * Time.deltaTime, 0);
            }
            else if (game.gravity == Vector2.up)
            {
                v.y = Mathf.Max(v.y - coeff * Time.deltaTime, 0);
            }
            else
            {
                v.y = Mathf.Min(v.y + coeff * Time.deltaTime, 0);
            }
        }
        rigidbody.velocity = MAX_SPEED * v;

        // Enterで弾発射
        bool returnPressed =
            Input.GetKey(KeyCode.Return) || returnButton.isPressed ||
            Input.GetKey(KeyCode.Space);
        if (returnPressed &&
            game.time - lastFiredTime >= FIRE_COOL_TIME)
        {
            FireBullet();
        }
    }

    public override void Die()
    {
        SetHP();
        base.Die();
    }

    protected override void PlayDamageSound()
    {
        Game game = Game.Get();
        game.audioSource.PlayOneShot(playerDamageSound, 0.8f);
    }

    protected override void PlayDeathSound()
    {
        Game game = Game.Get();
        game.audioSource.PlayOneShot(playerDeathSound, 0.6f);
    }

    private void FireBullet()
    {
        Game game = Game.Get();

        game.audioSource.PlayOneShot(bulletSound, 0.25f);
        string name = nameof(Bullet);
        Vector2 bulletDir;
        if (transform.eulerAngles.z == 0) bulletDir = new Vector2(0, 1);
        else if (transform.eulerAngles.z == 90) bulletDir = new Vector2(-1, 0);
        else if (transform.eulerAngles.z == 180) bulletDir = new Vector2(0, -1);
        else bulletDir = new Vector2(1, 0);
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject obj = Instantiate(
            prefab,
            transform.position + 0.8f * new Vector3(bulletDir.x, bulletDir.y, 0),
            Quaternion.Euler(0, 0, transform.eulerAngles.z + 90)
        );
        obj.name = name;
        obj.GetComponent<Rigidbody2D>().velocity = bulletDir;
        lastFiredTime = game.time;
    }

    private void SetHP()
    {

        // HPバー
        GameObject playerHPBar = GameObject.Find("PlayerHPBar");
        Slider hpSlider = playerHPBar.GetComponent<Slider>();
        hpSlider.value = (float)hp / MAX_HP;

        // HPテキスト
        GameObject playerHPText = GameObject.Find("PlayerHPText");
        Text hpText = playerHPText.GetComponent<Text>();
        hpText.text = $"{hp} / {MAX_HP}";
    }

    /**
    <summary>
    地面に足がついているかどうか
    </summary>
    */
    private bool IsGround()
    {
        Game game = Game.Get();
        List<Entity> entites = TouchingKinematics(game.gravity);
        entites = entites.Where(
            e =>
            !e.TryGetComponent(out Player _) &&
            !e.TryGetComponent(out Thorn _)
        ).ToList();
        return entites.Count > 0;
    }
}
