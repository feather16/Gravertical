using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
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

    public void OnMenuButtonPressed()
        => Game.OnMenuButtonPressed();

    public void OnContinueButtonPressed()
    {
        Game.Unpause();
    }

    public void OnHowToControlButtonPressed()
    {
        Game.howToControlWindowOpened = true;
        Game.Get().PlaySelectSound();
    }

    public void OnHowToControlBackButtonPressed()
    {
        Game.howToControlWindowOpened = false;
        Game.Get().PlayBackSound();
    }

    public void OnToTitleButtonPressed()
    {
        Game.toTitleWindowOpened = true;
        Game.Get().PlaySelectSound();
    }

    public void OnToTitleYesButtonPressed()
    {
        Game.BackToTitle();
    }

    public void OnToTitleNoButtonPressed()
    {
        Game.toTitleWindowOpened = false;
        Game.Get().PlayBackSound();
    }
}
