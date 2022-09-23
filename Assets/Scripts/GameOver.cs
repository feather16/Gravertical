using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    Game game;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        game = mainCamera.GetComponent<Game>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnRestartButtonPressed()
    {
        Game.RestartSameStage();
    }

    public void OnToTitleButtonPressed()
    {
        Game.BackToTitle();
    }
}
