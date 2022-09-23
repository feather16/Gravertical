using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChanger : Entity
{
    [SerializeField] private AudioClip gravityChangeSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Game game = Game.Get();

        // 触れても効果が無い場合は色を薄くする
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        bool noEffect
            = game.gravity == GetGravity();
        color.a = noEffect ? 0.55f : 1.0f;
        renderer.color = color;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        Game game = Game.Get();

        // 重力変更
        Vector2 prevGrav = new Vector2(game.gravity.x, game.gravity.y);
        if (collider.name == nameof(Player))
        {
            game.gravity = GetGravity();
        }
        if(game.gravity != prevGrav)
        {
            // 重力変更の音を鳴らす
            game.audioSource.PlayOneShot(gravityChangeSound, 0.7f);

            // 重力変更のエフェクトを出す
            string name = "GravityChangeEffect";
            GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
            GameObject obj = Instantiate(
                prefab,
                transform.position,
                Quaternion.Euler(0, 0, 0)
            );
            obj.name = name;
            obj.transform.parent = transform;

            // プレイヤーの速度を調整
            GameObject player = GameObject.Find(nameof(Player));
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            if(game.gravity == Vector2.right || game.gravity == Vector2.left)
            {
                playerRigidbody.velocity
                    = new Vector2(0, playerRigidbody.velocity.y);
            }
            if (game.gravity == Vector2.up || game.gravity == Vector2.down)
            {
                playerRigidbody.velocity
                    = new Vector2(playerRigidbody.velocity.x, 0);
            }
        }
    }

    private Vector2 GetGravity()
    {
        if (name.Contains("None"))
        {
            return Vector2.zero;
        }
        else
        {
            return Game.RotationToDirection(transform.eulerAngles);
        }
    }
}
