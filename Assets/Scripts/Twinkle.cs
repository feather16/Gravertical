using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twinkle : Entity
{
    private readonly float LIFE = 0.9f;
    private float bornTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Game game = Game.Get();
        bornTime = game.time;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Game game = Game.Get();
        if(game.time - bornTime > LIFE)
        {
            Die();
        }
        float lifeRate = (game.time - bornTime) / LIFE; // [0, 1]

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        color.a = Mathf.Sin(lifeRate * Mathf.PI);
        renderer.color = color;
    }
}
