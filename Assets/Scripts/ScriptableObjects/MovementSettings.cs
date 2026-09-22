using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float maxSpeed = 50f;
    public float acceleration = 20f;
    public float groundDeceleration = 20f;
    public float jumpForce = 1000f;
    public float groundAdhesion = 20f;
    [Range(0f, 1f)]
    public float airControl = 0.4f;
    
    [Header("Vault Settings")]
    public float vaultMaxDistance = 1.5f;
    public float vaultSpeed = 0.2f;
    
    [Header("Wall Run Settings")] 
    public float wallRunForce = 30f;
    public float maxWallRunSpeed = 12f;
    public float wallClimbCounterForce = 4f;
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 8f;
    public float wallCheckDistance = 0.8f;
    
    [Header("Sliding Settings")] 
    public float slideSpeedBoost = 5f;
    public float slideFriction = 5f;
    public float slideMaxDuration = 1.2f;
    public float crouchHeight = 1f;
    public float slideSlopeAcceleration = 20f;
}
