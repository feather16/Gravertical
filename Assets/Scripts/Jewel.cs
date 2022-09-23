using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : Entity
{
    [SerializeField] private AudioClip jewelGetSound;

    private readonly float TWINKLE_PERIOD = 1f;
    private float lastTwinkleTime = 0;

    /**<summary> ジュエルの色の候補 [0, 1]のRGB </summary>*/
    private readonly (float, float, float)[] COLORS =
    {
        (1, 0, 0),
        (0, 1, 0),
        (1, 1, 0),
        (0, 1, 1),
        (1, .2f, 1),
        (.5f, .2f, 1),
    };

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        var renderer = GetComponent<SpriteRenderer>();
        var color = renderer.color;
        var c = COLORS[bornPositionHash % COLORS.Length];
        color.r = c.Item1;
        color.g = c.Item2;
        color.b = c.Item3;
        renderer.color = color;

        Game game = Game.Get();
        float rate = ((bornPositionHash >> 4) & 0xf) / 16f;
        lastTwinkleTime = game.time - TWINKLE_PERIOD * rate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Game game = Game.Get();

        // 回転
        float rotRate = (0.25f * game.time + (bornPositionHash & 0xf) / 16f) % 1; // [0, 1)
        Vector3 angle = transform.eulerAngles;
        if(rotRate < 0.5f)
        {
            angle.y = 180 * (
                (1 - Mathf.Cos(rotRate * 2 * Mathf.PI)) / 2f
            );
        }
        else
        {
            angle.y = 180 * (
                (1 - Mathf.Cos((rotRate - 0.5f) * 2 * Mathf.PI)) / 2f
            ) + 180;
        }
        transform.eulerAngles = angle;
        
        // Twinkle
        if(game.time > lastTwinkleTime + TWINKLE_PERIOD)
        {
            lastTwinkleTime += TWINKLE_PERIOD;
            Func<float, float> f = x => (1 - Mathf.Cos(Mathf.PI * x)) / 2f;
            float xRate = f(UnityEngine.Random.value);
            float yRate = f(UnityEngine.Random.value);
            float x = transform.localScale.x * (-0.5f + xRate);
            float y = transform.localScale.y * (-0.5f + yRate);
            string name = "Twinkle";
            GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
            GameObject obj = Instantiate(
                prefab,
                transform.position + new Vector3(x, y, -1),
                Quaternion.Euler(0, 0, 0)
            );
            obj.name = name;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out Player _))
        {
            Damaged();
        }
    }

    public override void Die()
    {
        Game game = Game.Get();
        game.treasuresCollected++;
        game.audioSource.PlayOneShot(jewelGetSound, 1.25f);

        // エフェクト
        string name = "JewelEffect";
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject obj = Instantiate(
            prefab,
            transform.position + new Vector3(0, 0, -1),
            Quaternion.Euler(0, 0, 0)
        );
        obj.name = name;
        SpriteRenderer effectRenderer 
            = obj.GetComponent<SpriteRenderer>();
        effectRenderer.color
            = GetComponent<SpriteRenderer>().color;

        base.Die();
    }
}
