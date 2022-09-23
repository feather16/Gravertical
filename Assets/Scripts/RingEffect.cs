using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEffect : MonoBehaviour
{
    [SerializeField] private float MAX_RADIUS;
    [SerializeField] private float TIME;
    private float bornTime;

    // Start is called before the first frame update
    void Start()
    {
        Game game = Game.Get();
        bornTime = game.time;
    }

    // Update is called once per frame
    void Update()
    {
        Game game = Game.Get();

        // TIME•bŒo‚Á‚½‚çÁ‚¦‚é
        if (game.time - bornTime > TIME)
        {
            Destroy(gameObject);
        }

        float timeRate = (game.time - bornTime) / TIME;
        float scale = (1 - timeRate) * 1 + timeRate * MAX_RADIUS;
        transform.localScale = new Vector3(scale, scale, 1);
        var renderer = GetComponent<SpriteRenderer>();
        var color = renderer.color;
        color.a = 1 - timeRate;
        renderer.color = color;
    }
}
