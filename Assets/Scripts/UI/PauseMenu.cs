using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    public static PauseMenu instance;
    public PlayerMovement player;

    private void Start()
    {
        instance = this;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        panel.SetActive(true);
        CollectableUI collectableUI = gameObject.GetComponent<CollectableUI>();
        collectableUI.ColorCode();
        Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        panel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
}
