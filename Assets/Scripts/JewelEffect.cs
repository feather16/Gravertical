using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelEffect : Entity
{
    /**<summary> è¡Ç¶ÇÈÇ‹Ç≈ÇÃéûä‘ </summary>*/
    private readonly float LIFE = 0.9f;

    /**<summary> çÇÇ≥ÇÃç≈ëÂíl </summary>*/
    private readonly float MAX_HEIGHT = 1.3f;

    private float bornTime;
    private float originalY;

    /**<summary> ÉLÉâÉLÉâÇê∂ê¨Ç∑ÇÈé¸ä˙ </summary>*/
    private readonly float TWINKLE_PERIOD = 0.1f;

    private float lastTwinkleTime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Game game = Game.Get();
        bornTime = game.time;
        originalY = transform.position.y;

        float rate = ((bornPositionHash >> 4) & 0xf) / 16f;
        lastTwinkleTime = game.time - TWINKLE_PERIOD * rate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Game game = Game.Get();
        if (game.time - bornTime > LIFE)
        {
            Die();
        }
        float lifeRate = (game.time - bornTime) / LIFE; // [0, 1]

        // çÇÇ≥
        float height 
            = MAX_HEIGHT * 4 * lifeRate * (1 - lifeRate); // [0, MAX_HEIGHT]
        Vector2 pos = transform.position;
        pos.y = originalY + height;
        transform.position = pos;

        // âÒì]
        Vector3 angle = transform.eulerAngles;
        angle.y = (
            (game.time - bornTime) * 3f +
            (bornPositionHash & 0xf) / 16f
        ) * 360;
        transform.eulerAngles = angle;

        // Twinkle
        if (game.time > lastTwinkleTime + TWINKLE_PERIOD)
        {
            lastTwinkleTime += TWINKLE_PERIOD;
            Func<float, float> f = x => (1 - Mathf.Cos(Mathf.PI * x)) / 2f;
            float xRate = f(UnityEngine.Random.value);
            float yRate = f(UnityEngine.Random.value);
            float x = transform.localScale.x * (-0.5f + xRate);
            float y = transform.localScale.y * (-0.5f + yRate);
            string name = "Twinkle";
            GameObject prefab 
                = Resources.Load<GameObject>(@"Prefabs/" + name);
            GameObject obj = Instantiate(
                prefab,
                transform.position + new Vector3(x, y, -1),
                Quaternion.Euler(0, 0, 0)
            );
            obj.name = name;
        }
    }
}
