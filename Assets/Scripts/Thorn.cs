using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : Entity
{
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

        Vector3 angle = transform.eulerAngles;
        angle.z = (game.time * .5f * 360 + bornPositionHash % 360) % 360;
        transform.eulerAngles = angle;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Entity entity))
        {
            if (entity.thornDamage)
            {
                entity.Damaged();
            }
        }
    }
}
