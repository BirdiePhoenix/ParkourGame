using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private InputSystem_Actions inputActions;

    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamePadSensitivity = 0.1f;

    private float cameraPitch = 0f;
    private bool isSliding = false;
    Vector2 moveInput;
    Vector2 lookInput;

    public float maxSpeed = 50f;
    public float acceleration = 20f;
    public float groundDeceleration = 20f;
    [Range(0f, 1f)] public float airControl = 0.4f;
    
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction lookAction;
    private InputAction slideAction;
    private InputAction interactAction;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");
        slideAction = InputSystem.actions.FindAction("Crouch");
        interactAction = InputSystem.actions.FindAction("Interact");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        jumpAction.performed += JumpAction_performed;
        
        slideAction.performed += SlideAction_performed;
        slideAction.canceled += SlideAction_canceled;
        
        interactAction.performed += InteractAction_performed;
    }

    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Interact");
    }

    private void OnDisable()
    {
        jumpAction.performed -= JumpAction_performed;
        slideAction.performed -= SlideAction_performed;
        slideAction.canceled -= SlideAction_canceled;
        interactAction.performed -= InteractAction_performed;
    }

    private void Start()
    {
        rb.linearDamping = 0;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
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

    private void Update()
    {
        CameraHandling();
        
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, LayerMask.GetMask("Ground"));
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        Vector3 targetDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        if (moveInput.magnitude < 0.01f && isGrounded)
        {
            float speedToDrop = groundDeceleration * Time.deltaTime;
            if (currentHorizontalVelocity.magnitude <= speedToDrop)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            else
            {
                Vector3 brakeForce = currentHorizontalVelocity.normalized * -speedToDrop;
                rb.linearVelocity += brakeForce;
            }

            return;
        }

        float currentAcceleration = isGrounded ? acceleration : (acceleration * airControl);
        Vector3 forceToApply = targetDirection * currentAcceleration;
        
        float currentVelocityInInputDirection = Vector3.Dot(currentHorizontalVelocity, targetDirection);
        if (currentVelocityInInputDirection + (currentAcceleration * Time.fixedDeltaTime) > maxSpeed)
        {
            float availableSpeedRoom = maxSpeed - currentVelocityInInputDirection;
            availableSpeedRoom = Mathf.Max(0, availableSpeedRoom);
            forceToApply = targetDirection * (availableSpeedRoom / Time.fixedDeltaTime);
        }
        
        rb.AddForce(forceToApply, ForceMode.Force);
    }

    private void CameraHandling()
    {
        //float currentMouseSensitivity = inputActions.controlSchemes == "Gamepad" ? gamePadSensitivity : mouseSensitivity;
        lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        cameraPitch -= mouseY * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * (mouseX * mouseSensitivity));
    }   
}
