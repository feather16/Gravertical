using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GreenUFO : Entity
{
    /** <summary> 移動方向のベクトル </summary> */
    private Vector2 moveVec;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        if (TouchingRigidbodies(moveVec).Count > 0)
        {
            GameObject playerObject = GameObject.Find("Player");
            if(playerObject != null)
            {
                moveVec = 
                    (playerObject.transform.position - 
                    transform.position).normalized;
            }
            else
            {
                float theta = Random.Range(0, 2 * Mathf.PI);
                Vector2 v = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                moveVec = (v + 0.5f * moveVec).normalized;
            }
        }

        rigidbody.velocity = 2f * moveVec;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Player player))
        {
            player.Damaged();
        }
    }
}
