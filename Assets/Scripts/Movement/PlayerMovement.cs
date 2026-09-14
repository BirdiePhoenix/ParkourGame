using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    //private Vector2 moveInput;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float moveSpeed = 5f;

    private Camera mainCamera;
    private CharacterController characterController;

    private InputAction moveAction;
    private InputAction jumpAction;

    private float currentMoveSpeed = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private float lookAngle = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        currentMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(moveInput.x, rb.linearVelocity.y, moveInput.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log(context);
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }
        
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        //moveInput = context.ReadValue<Vector2>();
    }
}
