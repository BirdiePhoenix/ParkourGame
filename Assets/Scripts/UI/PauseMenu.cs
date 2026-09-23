using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    public InputActionAsset InputActions;

    public static PauseMenu instance;
    public PlayerMovement player;
    public InputAction pauseActionPlayer;
    public InputAction pauseActionUI;

    private void Start()
    {
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
    }
    public void PauseDisable()
    {
        player.paused = !player.paused;
        Close();

        InputActions.FindActionMap("UI").Disable();
        InputActions.FindActionMap("Player").Enable();
    }
}
