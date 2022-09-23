using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlueUFO : Entity
{
    public enum MoveType
    {
        x, y
    }
    [SerializeField] private MoveType moveType;

    /** <summary> �������̍��W </summary> */
    private float originalPos;

    /** <summary> �v���X����(1)�ɐi�ނ��}�C�i�X����(-1)�ɐi�ނ� </summary> */
    private int moveDir = 1;

    /** <summary> �ړ������̃x�N�g�� </summary> */
    private Vector2 moveVec
        => moveDir * (
        moveType == MoveType.x ? Vector2.right : Vector2.up
        );

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        originalPos
            = moveType == MoveType.x ?
            transform.position.y :
            transform.position.x;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Move();
    }

    private void Move()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        // �i�߂Ȃ��Ƃ�
        if (TouchingKinematics(moveVec).Count > 0)
        {
            // �ړ������𔽓]����
            moveDir *= -1;
        }

        rigidbody.velocity = 3f * moveVec;

        // �ړ������Ɛ����ȍ��W���͌Œ�
        Vector2 pos = transform.position;
        if (moveType == MoveType.x)
        {
            pos.y = originalPos;
        }
        else
        {
            pos.x = originalPos;
        }
        transform.position = pos;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // �v���C���[�ƏՓ˂���ƃv���C���[�Ƀ_���[�W
        if (collision.collider.TryGetComponent(out Player player))
        {
            player.Damaged();
        }
    }
}
