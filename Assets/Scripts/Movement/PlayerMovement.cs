using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

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
    private bool isVaulting = false;
    private bool isWallRunning = false;
    Vector2 moveInput;
    Vector2 lookInput;

    public float maxSpeed = 50f;
    public float acceleration = 20f;
    public float groundDeceleration = 20f;
    [Range(0f, 1f)] public float airControl = 0.4f;
    
    [Header("Vault Settings")] 
    public float vaultMaxDistance = 1.5f;
    public float vaultSpeed = 0.2f;
    public LayerMask obstacleMask;

    [Header("Raycast Positions")]
    [SerializeField] private Transform eyeLevel;
    [SerializeField] private Transform waistLevel;

    [Header("Wall Run Settings")] 
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallRunForce = 30f;
    [SerializeField] private float maxWallRunSpeed = 12f;
    [SerializeField] private float wallClimbCounterForce = 4f;
    [SerializeField] private float wallJumpUpForce = 7f;
    [SerializeField] private float wallJumpSideForce = 8f;

    [Header("Wall Detection")] 
    [SerializeField] private float wallCheckDistance = 0.8f;
    private RaycastHit leftWallHit, rightWallHit;
    private bool wallLeft, wallRight;
    
public InputAction moveAction;
public InputAction jumpAction;
public InputAction sprintAction;
public InputAction lookAction;
public InputAction slideAction;
public InputAction interactAction;

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
        if (!isGrounded)
            return;
        
        TryVault();
        WallJump();

        if (!isVaulting)
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
        CheckForWall();

        bool isMidAir = !Physics.CheckSphere(transform.position - new Vector3(0f, 1f, 0f), 0.3f, wallMask);

        if ((wallLeft || wallRight) && isMidAir)
        {
            StartWallRun();
        }
        else
        {
            StopWallRun();
        }   
        
        Movement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, wallMask);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, wallMask);
    }

    private void Movement()
    {
        if (isVaulting)
            return;
        
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
    
    private void TryVault()
    {
        if (isVaulting)
            return;

        if (Physics.Raycast(waistLevel.position, transform.forward, out RaycastHit wallHit, vaultMaxDistance,
                obstacleMask))
        {
            if (!Physics.Raycast(eyeLevel.position, transform.forward, vaultMaxDistance, obstacleMask))
            {
                Vector3 vaultTargetPos =  wallHit.point + (transform.forward * 2f) + (Vector3.up * 1f);
                
                StartCoroutine(ExecuteVault(vaultTargetPos));
            }
        }
    }

    private IEnumerator ExecuteVault(Vector3 targetPos)
    {
        isVaulting = true;
        rb.isKinematic = true;
        
        Vector3 startPos = transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < vaultSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed/vaultSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos;
        
        rb.isKinematic = false;
        isVaulting = false;
    }

    private void StartWallRun()
    {
        if (!isWallRunning)
        {
            isWallRunning = true;
            rb.useGravity = false;
        }
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbCounterForce * Time.fixedDeltaTime, rb.linearVelocity.z);
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if (Vector3.Dot(transform.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }
        

        if (rb.linearVelocity.magnitude < maxWallRunSpeed)
        {
            rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        }

    }

    private void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            rb.useGravity = true;
        }
    }

    private void WallJump()
    {
        if (!isWallRunning)
            return;
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        
        Vector3 jumpDirection = (transform.up * wallJumpUpForce) + (wallNormal * wallJumpSideForce);

        StopWallRun();
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(jumpDirection, ForceMode.VelocityChange);
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
