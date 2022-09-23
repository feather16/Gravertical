using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected int MAX_HP = -1;
    [SerializeField] protected int hp = -1; // デバッグのためにSerializeFieldにしてる
    
    /**<summary> トゲによるダメージを受けるか </summary>*/
    [SerializeField] public bool thornDamage = false;
    
    [SerializeField] public bool isCreature = false;
    [SerializeField] public bool hasTreasure = false;

    /**<summary> 無敵時間 </summary>*/
    protected readonly float INVI_TIME = .5f;

    protected float lastDamagedTime;

    protected uint bornPositionHash { get; private set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        bornPositionHash = GetHashFromPosition();

        if(TryGetComponent(out SpriteRenderer renderer))
        {
            renderer.sortingOrder = 0;
        }
        hp = MAX_HP;
        lastDamagedTime = -10000;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Game game = Game.Get();

        // 無敵時間中の点滅
        if (isCreature)
        {
            int blinkingCount = 5;
            float phase = (game.time - lastDamagedTime) / INVI_TIME;
            if (phase < 1)
            {
                bool disappeared
                    = (int)(phase * blinkingCount * 2) % 2 == 0;
                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                Color color = renderer.color;
                color.a = disappeared ? 0 : 1;
                renderer.color = color;
            }
        }
    }

    public virtual bool Damaged(int damage = 1)
    {
        Game game = Game.Get();
        bool damaged = false;
        bool die = false;

        if (
            hp != -1 && // HPの概念がある
            game.time - lastDamagedTime >= INVI_TIME // 前回のダメージから一定時間経過
            )
        {
            hp = Mathf.Max(hp - damage, 0);
            if (hp > 0)
            {
                lastDamagedTime = game.time;
                if (isCreature)
                {
                    PlayDamageSound();
                }
            }
            else
            {
                die = true;
            }
            damaged = true;
        }

        if(damaged && isCreature)
        {
            // ダメージの数値を表示
            GenerateDamageNumberEffect(damage);
        }

        if (die)
        {
            Die();
        }

        return damaged;
    }

    public virtual void Die()
    {
        hp = 0;

        Game game = Game.Get();
        if (isCreature)
        {
            PlayDeathSound();
        }

        // お宝を持っていれば
        if (hasTreasure)
        {
            // お宝を落とす
            string name = "Jewel";
            GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
            GameObject obj = Instantiate(
                prefab,
                transform.position,
                Quaternion.Euler(0, 0, 0)
            );
            obj.name = name;
        }

        Destroy(gameObject);
    }

    protected void GenerateDamageNumberEffect(int damage)
    {
        float radius = Random.Range(0.25f, 0.5f);
        float theta = Random.Range(0, 2 * Mathf.PI);

        // 中心からの位置ズレ
        Vector2 pos = radius * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        
        // ダメージの値のエフェクトを生成
        string name = "DamageNumberEffect";
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject obj = Instantiate(
            prefab,
            transform.position + (Vector3)pos,
            Quaternion.Euler(0, 0, 0)
        );
        obj.name = name;
        DamageNumberEffect damageNumber
            = obj.GetComponent<DamageNumberEffect>();
        damageNumber.number = damage;
    }

    protected virtual void PlayDamageSound()
    {
        Game game = Game.Get();
        game.audioSource.PlayOneShot(
            game.enemyDamageSound,
            0.55f
        );
    }

    protected virtual void PlayDeathSound()
    {
        Game game = Game.Get();
        game.audioSource.PlayOneShot(
            game.enemyDeathSound,
            0.45f
        );
    }

    /**
    <summary>
    現在値から符号無し32ビットの乱数を生成
    </summary>
    */
    private uint GetHashFromPosition()
    {
        byte[] xBytes = System.BitConverter.GetBytes(transform.position.x);
        byte[] yBytes = System.BitConverter.GetBytes(transform.position.y);
        var md5 = new MD5CryptoServiceProvider();
        byte[] hash = md5.ComputeHash(xBytes.Concat(yBytes).ToArray());
        return (uint)System.BitConverter.ToUInt64(hash);
    }

    // dir方向に触れているEntityを返す
    protected List<Entity> TouchingEntites(Vector2 dir)
    {
        if(dir != Vector2.zero)
        {
            dir = dir.normalized;
        }

        float width = transform.localScale.x;
        float height = transform.localScale.y;

        Vector3 left = Quaternion.Euler(0, 0, -90) * dir;
        Vector3 right = Quaternion.Euler(0, 0, 90) * dir;
        Vector3 down = dir;

        Vector3 leftStartPoint = transform.position + left * 0.45f * width + down * height * 0.5f;
        Vector3 rightStartPoint = transform.position + right * 0.45f * width + down * height * 0.5f;
        Vector3 leftEndPoint = leftStartPoint + down * height * 1f / 64f;
        Vector3 rightEndPoint = rightStartPoint + down * height * 1f / 64f;
        Debug.DrawRay(leftStartPoint, leftEndPoint - leftStartPoint, Color.red);
        Debug.DrawRay(rightStartPoint, rightEndPoint - rightStartPoint, Color.red);
        Debug.DrawRay(leftEndPoint, rightEndPoint - leftEndPoint, Color.red);

        RaycastHit2D[] hitsLeft, hitsRight, hitsCenter;
        hitsLeft = Physics2D.LinecastAll(leftStartPoint, leftEndPoint);
        hitsRight = Physics2D.LinecastAll(leftStartPoint, rightEndPoint);
        hitsCenter = Physics2D.LinecastAll(leftEndPoint, rightEndPoint);
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        hits.AddRange(hitsLeft);
        hits.AddRange(hitsRight);
        hits.AddRange(hitsCenter);
        IEnumerable<GameObject> objects
            = hits.Select(h => h.collider.gameObject);
        objects = objects.Distinct();
        objects = objects.Where(o => o.TryGetComponent(out Entity _));
        return objects.Select(o => o.GetComponent<Entity>()).ToList();
    }

    // dir方向に触れているRigidbodyを持つEntityを返す
    protected List<Entity> TouchingRigidbodies(Vector2 dir)
    {
        return TouchingEntites(dir)
            .Where(e => e.TryGetComponent(out Rigidbody2D _))
            .ToList();
    }

    // dir方向に触れているKinematicなEntityを返す
    protected List<Entity> TouchingKinematics(Vector2 dir)
    {
        return TouchingRigidbodies(dir)
            .Where(e => e.GetComponent<Rigidbody2D>().isKinematic)
            .ToList();
    }
}
