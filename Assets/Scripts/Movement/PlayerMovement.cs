using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private InputSystem_Actions inputActions;
    //private Vector2 moveInput;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamePadSensitivity = 0.1f;
    private float cameraPitch = 0f;
    private bool isSliding = false;


    private Camera mainCamera;
    
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction lookAction;
    public InputAction slideAction;

    private float currentMoveSpeed = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private float lookAngle = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");
        slideAction = InputSystem.actions.FindAction("Crouch");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        jumpAction.performed += JumpAction_performed;
        slideAction.performed += SlideAction_performed;
        slideAction.canceled += SlideAction_canceled;
    }

    private void OnDisable()
    {
        jumpAction.performed -= JumpAction_performed;
        slideAction.performed -= SlideAction_performed;
    }

    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }
    }

    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        if(!isGrounded)
            return;
        Debug.Log(obj); 

        transform.localScale *= 0.5f;
        isSliding = true;
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        if (isSliding)
        {
            transform.localScale *= 2f;
        }
        isSliding = false;

    }

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        //characterController = GetComponent<CharacterController>();
        currentMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, LayerMask.GetMask("Ground"));
        Movement();
        CameraHandling();
    }

    private void Movement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        rb.MovePosition(rb.position + ((transform.forward * moveInput.y) + (transform.right * moveInput.x)) * (moveSpeed * Time.deltaTime));
    }

    private void CameraHandling()
    {
        //float currentMouseSensitivity = inputActions.controlSchemes == "Gamepad" ? gamePadSensitivity : mouseSensitivity;
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        cameraPitch -= mouseY * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);
    }   
}
