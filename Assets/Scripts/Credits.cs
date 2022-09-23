using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hypertext;

public class Credits : MonoBehaviour
{
    private static readonly string REGEX_URL = @"https?://(?:[!-~]+\.)+[!-~]+";
    [SerializeField] RegexHypertext creditsText = default;

    [SerializeField] private AudioClip backSound;

    private AudioSource audioSource;

    private static void OpenURL(string url)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_WEBGL
        Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
#else
        Application.OpenURL(url);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // クレジットを表示
        TextAsset creditsFile = Resources.Load("Data/credits") as TextAsset;
        string creditsTextStr = creditsFile.text;
        creditsText.text = creditsTextStr;
        var lines = creditsTextStr.Split('\n').Length;
        creditsText.fontSize = 160 / (lines + 1);

        // URLをクリックすると移動するように設定
        creditsText.OnClick(
            REGEX_URL, 
            new Color32(156, 190, 207, 255), // 水色
            url =>
        {
            OpenURL(url);
            //Debug.Log("Open " + url);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OnBackButtonPressed();
        }
    }

    public void OnBackButtonPressed()
    {
        audioSource.PlayOneShot(backSound, 0.6f);
        Invoke(nameof(BackToTitle), 0.2f);
    }

    private void BackToTitle()
    {
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single);
    }
}
