using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerMovement player, MovementSettings settings) : base(player, settings)
    {
    }

    public override void FixedUpdate()
    {
        Vector3 currentHorizontalVelocity = new Vector3(player.RigidBody.linearVelocity.x, 0f, player.RigidBody.linearVelocity.z);
        
        Vector3 targetDirection = (player.transform.forward * player.MoveInput.y + player.transform.right * player.MoveInput.x);

        float inputMagnitude = Mathf.Clamp01(targetDirection.magnitude);

        if (player.IsGrounded)
        {
            targetDirection = Vector3.ProjectOnPlane(targetDirection, player.GroundNormal).normalized;
        }
        else
        {
            targetDirection.Normalize();
        }
        
        if (targetDirection.sqrMagnitude < 0.001f && player.IsGrounded)
        {
            Vector3 movementVelocity = Vector3.ProjectOnPlane(player.RigidBody.linearVelocity, player.GroundNormal);
            
            float speedToDrop = settings.groundDeceleration * Time.deltaTime;
            if (currentHorizontalVelocity.magnitude <= speedToDrop)
            {
                player.RigidBody.linearVelocity -= movementVelocity;
            }
            else
            {
                player.RigidBody.linearVelocity -= movementVelocity * speedToDrop;
            }

            return;
        }

        float diagonalScale = 1f;
        if (Mathf.Abs(player.MoveInput.x) > 0.1f && Mathf.Abs(player.MoveInput.y) > 0.1f)
        {
            diagonalScale = 1.414f;
        }

        float currentAcceleration = player.IsGrounded ? settings.acceleration : (settings.acceleration * settings.airControl);
        Vector3 forceToApply = targetDirection * (currentAcceleration * diagonalScale * inputMagnitude);
        
        float currentVelocityInInputDirection = Vector3.Dot(currentHorizontalVelocity, targetDirection);
        if (currentVelocityInInputDirection + ((currentAcceleration * diagonalScale) * Time.fixedDeltaTime) > settings.maxSpeed)
        {
            float availableSpeedRoom = Mathf.Max(0, settings.maxSpeed - currentVelocityInInputDirection);
            forceToApply = targetDirection * (availableSpeedRoom / Time.fixedDeltaTime);
        }
        
        player.RigidBody.AddForce(forceToApply, ForceMode.Force);
    }
}
