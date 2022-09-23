using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Octopus : Entity
{
    [SerializeField] private Sprite image1, image2;

    private float rotCoolTime = 0;

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

        Move();

        // ���[�V����
        float motionRate = (2f * game.time + (bornPositionHash & 0xf) / 16f) % 1; // [0, 1)
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = motionRate < 0.5f ? image1 : image2;
    }

    private void Move()
    {
        if (rotCoolTime > 0)
        {
            rotCoolTime = Mathf.Max(rotCoolTime - Time.deltaTime, 0);
        }

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        // �ǂ̕���
        Vector2 wallDir = -Game.RotationToDirection(transform.eulerAngles);
        
        List<Entity> floorKinematics = TouchingKinematics(wallDir);

        // ���ɐG��Ă��Ȃ��Ƃ�
        if (floorKinematics.Count == 0)
        {
            rigidbody.velocity = wallDir * 2f;

            // ���ɂԂ���
            Vector2 sideDir = Quaternion.Euler(0, 0, 90) * wallDir;
            if (TouchingKinematics(sideDir).Count > 0 && rotCoolTime == 0)
            {
                // �ǂɂ�����(�����v���)
                Vector3 angle = transform.eulerAngles;
                angle.z = (angle.z + 90) % 360;
                transform.eulerAngles = angle;
                rotCoolTime = 0.2f;
            }
        }

        // ���ɐG��Ă���Ƃ�
        else
        {
            Vector2 moveDir = Quaternion.Euler(0, 0, -90) * wallDir;

            // �i�s�ł���Ƃ�
            if (TouchingKinematics(moveDir).Count == 0)
            {
                // �i�s
                rigidbody.velocity = moveDir * 2f;
                rigidbody.AddForce(wallDir * 0.1f);
            }
            // �i�s�ł��Ȃ��Ƃ�
            else
            {
                if (rotCoolTime == 0)
                {
                    // ������ς���(���v���)
                    transform.eulerAngles = Game.DirectionToRotation(-moveDir);
                    rotCoolTime = 0.2f;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Player player))
        {
            player.Damaged();
        }
    }
}
