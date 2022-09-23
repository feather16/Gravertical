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

    /** <summary> 生成時の座標 </summary> */
    private float originalPos;

    /** <summary> プラス方向(1)に進むかマイナス方向(-1)に進むか </summary> */
    private int moveDir = 1;

    /** <summary> 移動方向のベクトル </summary> */
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

        // 進めないとき
        if (TouchingKinematics(moveVec).Count > 0)
        {
            // 移動方向を反転する
            moveDir *= -1;
        }

        rigidbody.velocity = 3f * moveVec;

        // 移動方向と垂直な座標軸は固定
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
        // プレイヤーと衝突するとプレイヤーにダメージ
        if (collision.collider.TryGetComponent(out Player player))
        {
            player.Damaged();
        }
    }
}
