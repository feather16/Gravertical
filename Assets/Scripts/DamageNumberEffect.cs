using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumberEffect : Entity
{
    /**<summary> ダメージの値 </summary>*/
    public int number = -1;

    private TextMeshPro text;

    /**<summary> 何秒間アクションするか </summary>*/
    private readonly float LIFE = 0.9f;

    /**<summary> アクションが終わってから何秒残るか </summary>*/
    private readonly float REMAINING = 0.5f;

    /**<summary> 高さの最大値 </summary>*/
    private readonly float MAX_HEIGHT = 1.3f;

    /**<summary> 左右のブレの最大値 </summary>*/
    private readonly float LR_DIFF_MAX = 0.2f;

    /**<summary> 左右のブレ </summary>*/
    private float lrDiff;

    private float bornTime;
    private Vector2 originalPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        text = GetComponent<TextMeshPro>();

        Game game = Game.Get();
        bornTime = game.time;

        originalPos = transform.position;

        float rate = ((bornPositionHash >> 5) & 0x1f) / 32f;
        lrDiff = LR_DIFF_MAX * rate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // 数字の設定
        if (number != -1)
        {
            text.text = number.ToString();
        }
        else
        {
            text.text = "";
        }

        Game game = Game.Get();
        if (game.time - bornTime > LIFE)
        {
            if (game.time - bornTime > LIFE + REMAINING)
            {
                Die();
            }
            else
            {
                return;
            }
        }
        float lifeRate = (game.time - bornTime) / LIFE; // [0, 1]

        // 高さ
        // 放物線を描く
        float height
            = MAX_HEIGHT * 4 * lifeRate * (1 - lifeRate); // [0, MAX_HEIGHT]
        Vector2 pos = transform.position;
        pos.x = originalPos.x + lifeRate * lrDiff;
        pos.y = originalPos.y + height;
        transform.position = pos;
    }
}
