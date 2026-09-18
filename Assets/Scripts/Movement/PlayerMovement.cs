using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuration Asset")] 
    [SerializeField] private MovementSettings settings;
    
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private InputSystem_Actions inputActions;
    
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction lookAction;
    public InputAction slideAction;
    public InputAction interactAction;
    public InputAction pauseAction;
    
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamePadSensitivity = 0.1f;
    
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask wallMask;
    

    [Header("Raycast Positions")]
    [SerializeField] private Transform eyeLevel;
    [SerializeField] private Transform waistLevel;
    
    [Header("Wall Detection")] 
    private RaycastHit leftWallHit, rightWallHit;
    private bool wallLeft, wallRight;
    
    private float cameraPitch = 0f;
    private bool isGrounded;
    public bool grounded => isGrounded;
    private bool isSliding = false;
    public bool IsSliding => isSliding;
    private bool isVaulting = false;
    public bool IsVaulting => isVaulting;
    private bool isWallRunning = false;
    public bool IsWallRunning => isWallRunning;
    private float slideTimer = 0f;
    private Vector3 slideDirection;
    private float originalHeight;
    private Vector3 originalCameraLocalPos;
    
    private Vector2 moveInput;
    private Vector2 lookInput;

    public bool paused = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");
        slideAction = InputSystem.actions.FindAction("Crouch");
        interactAction = InputSystem.actions.FindAction("Interact");
        pauseAction = InputSystem.actions.FindAction("Pause");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalHeight = capsuleCollider.height;
        originalCameraLocalPos = cameraTransform.localPosition;
    }

    private void OnEnable()
    {
        jumpAction.performed += JumpAction_performed;
        
        slideAction.performed += SlideAction_performed;
        slideAction.canceled += SlideAction_canceled;
        
        interactAction.performed += InteractAction_performed;
        pauseAction.performed += PauseAction_performed;
    }

    private void PauseAction_performed(InputAction.CallbackContext obj)
    {
        paused = true;
        PauseMenu.instance.player = this;
        PauseMenu.instance.PauseGame();
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
        pauseAction.performed -= PauseAction_performed;
    }

    private void Start()
    {
        rb.linearDamping = 1;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        if (paused != true) { CameraHandling(); }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, LayerMask.GetMask("Ground"));
        
        CheckForWall();

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f || !isGrounded)
            {
                StopSlide();
            }
        }
        
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
        
    }

    private void FixedUpdate()
    {
        if (isVaulting)
            return;

        if (isSliding)
        {
            ExecuteSlideMovement();
            return;
        }

        bool isMidAir = !Physics.CheckSphere(transform.position - new Vector3(0f, 1f, 0f), 0.3f, wallMask);

        if ((wallLeft || wallRight) && isMidAir)
        {
            StartWallRun();
        }
        else
        {
            StopWallRun();
            Movement();
        }   
        
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, settings.wallCheckDistance, wallMask);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, settings.wallCheckDistance, wallMask);
    }

    private void Movement()
    {
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        Vector3 targetDirection = (transform.forward * moveInput.y + transform.right * moveInput.x);

        float inputMagnitude = Mathf.Clamp01(targetDirection.magnitude);
        
        if (targetDirection.sqrMagnitude < 0.001f && isGrounded)
        {
            float speedToDrop = settings.groundDeceleration * Time.deltaTime;
            if (currentHorizontalVelocity.magnitude <= speedToDrop)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
            else
            {
                rb.linearVelocity += currentHorizontalVelocity.normalized * -speedToDrop;
            }

            return;
        }

        float diagonalScale = 1f;
        if (Mathf.Abs(moveInput.x) > 0.1f && Mathf.Abs(moveInput.y) > 0.1f)
        {
            diagonalScale = 1.414f;
        }

        float currentAcceleration = isGrounded ? settings.acceleration : (settings.acceleration * settings.airControl);
        Vector3 forceToApply = targetDirection * (currentAcceleration * diagonalScale * inputMagnitude);
        
        float currentVelocityInInputDirection = Vector3.Dot(currentHorizontalVelocity, targetDirection);
        if (currentVelocityInInputDirection + ((currentAcceleration * diagonalScale) * Time.fixedDeltaTime) > settings.maxSpeed)
        {
            float availableSpeedRoom = Mathf.Max(0, settings.maxSpeed - currentVelocityInInputDirection);
            forceToApply = targetDirection * (availableSpeedRoom / Time.fixedDeltaTime);
        }
        
        rb.AddForce(forceToApply, ForceMode.Force);
    }
    
    private void TryVault()
    {
        if (isVaulting)
            return;

        if (Physics.Raycast(waistLevel.position, transform.forward, out RaycastHit wallHit, settings.vaultMaxDistance,
                obstacleMask))
        {
            if (!Physics.Raycast(eyeLevel.position, transform.forward, settings.vaultMaxDistance, obstacleMask))
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

        while (timeElapsed < settings.vaultSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed/settings.vaultSpeed);
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
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -settings.wallClimbCounterForce * Time.fixedDeltaTime, rb.linearVelocity.z);
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if (Vector3.Dot(transform.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }
        

        if (rb.linearVelocity.magnitude < settings.maxWallRunSpeed)
        {
            rb.AddForce(wallForward * settings.wallRunForce, ForceMode.Force);
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
        
        Vector3 jumpDirection = (transform.up * settings.wallJumpUpForce) + (wallNormal * settings.wallJumpSideForce);

        StopWallRun();
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(jumpDirection, ForceMode.VelocityChange);
    }
    
    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (isWallRunning)
            WallJump();
            
        if (!isGrounded)
            return;
        
        TryVault();

        if (!isVaulting)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, settings.jumpForce, rb.linearVelocity.z);
        }        
        
    }

    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (currentHorizontalVelocity.magnitude > (settings.maxSpeed * 0.7f) && isGrounded)
        {
            StartSlide(currentHorizontalVelocity);
        }
        else
        {
            SetCapsuleHeight(settings.crouchHeight);
        }
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        if (isSliding)
        {
            StopSlide();
        }

        SetCapsuleHeight(originalHeight);
    }

    private void StartSlide(Vector3 currentHorizontalVelocity)
    {
        isSliding = true;
        slideTimer = settings.slideMaxDuration;
        
        slideDirection = currentHorizontalVelocity.normalized;
        
        SetCapsuleHeight(settings.crouchHeight);
        
        rb.AddForce(slideDirection * settings.slideSpeedBoost, ForceMode.VelocityChange);
    }

    private void ExecuteSlideMovement()
    {
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float speedToDrop = settings.slideFriction * Time.fixedDeltaTime;

        if (currentHorizontalVelocity.magnitude < speedToDrop)
        {
            rb.linearVelocity =  new Vector3(0f, rb.linearVelocity.y, 0f);
            StopSlide();
        }
        else
        {
            rb.linearVelocity += currentHorizontalVelocity.normalized * -speedToDrop;
        }
        
        Vector3 steeringInput = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        rb.AddForce(steeringInput * (settings.acceleration * 0.15f), ForceMode.Force);
    }

    private void StopSlide()
    {
        isSliding = false;
    }

    private void SetCapsuleHeight(float targetHeight)
    {
        capsuleCollider.height = targetHeight;
        
        float heightDifference = originalHeight - targetHeight;
        cameraTransform.localPosition = originalCameraLocalPos - new Vector3(0f, heightDifference * 0.5f, 0f);
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
