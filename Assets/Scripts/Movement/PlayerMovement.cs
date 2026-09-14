using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;
    [SerializeField] private float jumpForce = 100f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
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
        moveInput = context.ReadValue<Vector2>();
    }
}
