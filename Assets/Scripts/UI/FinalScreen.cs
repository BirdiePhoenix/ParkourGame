using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class FinalScreen : MonoBehaviour
{
    public GameObject screen;
    public TextMeshProUGUI timetext;
    public PlayerMovement player;

    public void Awaken(string time)
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        }
        player.paused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        screen.SetActive(true);
        Time.timeScale = 0f;
        timetext.text = ($"Final Time: {time}");
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
