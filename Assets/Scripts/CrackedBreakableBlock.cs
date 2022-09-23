using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBreakableBlock : Entity
{
    [SerializeField] private AudioClip destroyedSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // ランダムで180度回転とフリップをする
        Vector3 ang = transform.eulerAngles;
        ang.z = Random.Range(0, 2) == 0 ? 0 : 180;
        transform.eulerAngles = ang;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = Random.Range(0, 2) == 0;
        spriteRenderer.flipY = Random.Range(0, 2) == 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        // ブロックが壊れる音を鳴らす
        Game game = Game.Get();
        game.audioSource.PlayOneShot(destroyedSound, 0.8f);

        base.Die();
    }
}
