using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hypertext;

public class HowTo : MonoBehaviour
{
    private static readonly string REGEX_URL = @"https?://(?:[!-~]+\.)+[!-~]+";
    [SerializeField] RegexHypertext howtoText = default;

    [SerializeField] private AudioClip backSound;

    private AudioSource audioSource;

    private int page = 1;

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
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OnBackButtonPressed();
        }
    }

    public void OnChangeButtonPressed()
    {
        if (page == 1) page = 2;
        else page = 1;
        SetText();

        GameObject canvas = GameObject.Find("Canvas");
        Text buttonText
            = canvas.transform
            .Find("ChangeButton")
            .Find("Text")
            .gameObject
            .GetComponent<Text>();
        buttonText.text = page == 1 ? "1 / 2" : "2 / 2";
    }

    public void OnBackButtonPressed()
    {
        audioSource.PlayOneShot(backSound, 0.6f);
        Invoke(nameof(BackToTitle), 0.2f);
    }

    private void SetText()
    {
        string textStr;
        if (page == 1)
        {
            TextAsset gamerulesFiles = Resources.Load("Data/gamerules") as TextAsset;
            textStr = gamerulesFiles.text;
            var lines = textStr.Split('\n').Length;
            howtoText.fontSize = 160 / (lines + 1);
        }
        else
        {
            TextAsset creditsFile = Resources.Load("Data/howto") as TextAsset;
            textStr = creditsFile.text;
            var lines = textStr.Split('\n').Length;
            howtoText.fontSize = 230 / (lines + 1);
        }
        howtoText.text = textStr;
        howtoText.OnClick(
            REGEX_URL,
            new Color32(156, 194, 203, 255),
            url =>
            {
                OpenURL(url);
            });
    }

    private void BackToTitle()
    {
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single);
    }
}
