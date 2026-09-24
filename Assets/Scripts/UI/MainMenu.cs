using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame(int num)
    {
        SceneManager.LoadScene(num);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
