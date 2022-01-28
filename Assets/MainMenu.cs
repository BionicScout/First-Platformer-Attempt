using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public static int currentScene;
    public void A_StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void A_ExitButton()
    {
        Application.Quit();
    }

    public void A_BacktoMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void A_LoadScene()
    {
        SceneManager.LoadScene(currentScene);
    }

    public void UpdateCurrentScene(int i)
    {
        currentScene = i;
    }
}
