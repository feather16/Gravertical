using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Clear : MonoBehaviour
{
    Game game;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        game = mainCamera.GetComponent<Game>();

        // 次のステージがあるか
        bool hasNextStage 
            = Game.GetCurrentStageNumber() + 1 <= Game.GetNumStages();

        // 次のステージがあれば、次のステージに進むボタンを有効にする
        GameObject canvas = GameObject.Find("Canvas");
        UnityEngine.UI.Button nextButton = canvas.transform
            .Find("ClearPanel")
            .Find("NextButton")
            .GetComponent<UnityEngine.UI.Button>();
        nextButton.interactable = hasNextStage;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnNextStageButtonPressed()
    {
        Game.ToNextStage();
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
