using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    public InputActionAsset InputActions;

    public float fix2 = 0;

    public static PauseMenu instance;
    public PlayerMovement player;
    public InputAction pauseActionPlayer;
    public InputAction pauseActionUI;

    private void Start()
    {
        fix2 = 0;
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        instance = this;
        pauseActionPlayer = InputSystem.actions.FindAction("Player/Pause");
        pauseActionUI = InputSystem.actions.FindAction("UI/Pause");
    }

    private void Update()
    {
        if (pauseActionPlayer.WasPressedThisFrame())
        {
            PauseToggle();
        }
        if (pauseActionUI.WasPressedThisFrame())
        {
            PauseDisable();
        }
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
        Time.timeScale = 1f;
        Cursor.visible = false;
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PauseToggle()
    {
        player.paused = !player.paused;
        PauseGame();

        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Enable();
        fix2 = 1;
        Debug.Log("1");
    }
    public void PauseDisable()
    {
        if (fix2 == 1)
        {
            player.paused = !player.paused;
            Close();

            InputActions.FindActionMap("UI").Disable();
            InputActions.FindActionMap("Player").Enable();
            fix2 = 0;
            Debug.Log("2");
        }
    }
    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        fix2 = 0;
        player.paused = !player.paused;

        InputActions.FindActionMap("UI").Disable();
        InputActions.FindActionMap("Player").Enable();
        SceneManager.LoadScene(0);
    }
}
