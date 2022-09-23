using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Entity
{
    [SerializeField] private float SPEED;

    /** <summary> 初速度 </summary> */
    private Vector2 originalVelocity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // 速度をSPEEDにする
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity *= SPEED / rigidbody.velocity.magnitude;

        originalVelocity = rigidbody.velocity;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // 速度ベクトルは初速度で固定
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = originalVelocity;

        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;

        // Bullet同士が衝突した場合は無視
        if(obj.TryGetComponent(out Bullet _))
        {
            return;
        }

        // EntityかつPlayerで無ければ
        Entity entity;
        if(
            !obj.TryGetComponent(out Player _) && 
            obj.TryGetComponent(out entity))
        {
            // Entityにダメージを与える
            entity.Damaged(1);
        }

        // 自分もダメージを受ける
        Damaged();
    }

    public override void Die()
    {
        // 弾が消滅するエフェクトを出す
        string name = "BulletEffect";
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject obj = Instantiate(
            prefab,
            transform.position,
            Quaternion.Euler(0, 0, 0)
        );
        obj.name = name;

        base.Die();
    }
}
