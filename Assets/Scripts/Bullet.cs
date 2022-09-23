using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Entity
{
    [SerializeField] private float SPEED;

    /** <summary> �����x </summary> */
    private Vector2 originalVelocity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // ���x��SPEED�ɂ���
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity *= SPEED / rigidbody.velocity.magnitude;

        originalVelocity = rigidbody.velocity;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // ���x�x�N�g���͏����x�ŌŒ�
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = originalVelocity;

        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;

        // Bullet���m���Փ˂����ꍇ�͖���
        if(obj.TryGetComponent(out Bullet _))
        {
            return;
        }

        // Entity����Player�Ŗ������
        Entity entity;
        if(
            !obj.TryGetComponent(out Player _) && 
            obj.TryGetComponent(out entity))
        {
            // Entity�Ƀ_���[�W��^����
            entity.Damaged(1);
        }

        // �������_���[�W���󂯂�
        Damaged();
    }

    public override void Die()
    {
        // �e�����ł���G�t�F�N�g���o��
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
