using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] public float fadeInTime = 0.5f;
    private Image fadeInImage;
    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject canvasObject = GameObject.Find("Canvas");

        // フェード用の画像を作成
        string name = "FadeImage";
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject imageObject = Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.Euler(0, 0, 0)
        );
        imageObject.name = name;
        imageObject.transform.parent = canvasObject.transform;
        imageObject.transform.localPosition = Vector3.zero;
        fadeInImage = imageObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        // アルファ値を計算
        float alpha = Mathf.Clamp(
            1 - time / fadeInTime, 
            0, 
            1
        );

        // アルファ値を変更
        Color color = fadeInImage.color;
        color.a = alpha;
        fadeInImage.color = color;
    }
}
