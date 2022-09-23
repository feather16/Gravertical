using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private AudioClip gameStartSound;
    [SerializeField] private AudioClip selectSound;

    private AudioSource audioSource;

    private string stageName;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStageButtonPressed(string stageName)
    {
        this.stageName = stageName;
        audioSource.PlayOneShot(gameStartSound, 0.8f);
        Invoke(nameof(GameStart), 0.2f);
    }

    public void OnBackToTitleButtonPressed()
    {
        audioSource.PlayOneShot(selectSound, 0.4f);
        Invoke(nameof(BackToTitle), 0.2f);
    }

    private void GameStart()
    {
        SceneManager.LoadScene(
            $"Scenes/MainStage/{stageName}", 
            LoadSceneMode.Single
        );
    }

    private void BackToTitle()
    {
        SceneManager.LoadScene("Scenes/Title", LoadSceneMode.Single);
    }
}
