using UnityEngine;
using UnityEngine.XR;

public class PlayerSlideState : PlayerState
{
    private float slideAirTimer;
    private float slideTimer;
    private Vector3 slideDirection;
    
    
    public PlayerSlideState(PlayerMovement player, MovementSettings settings) : base(player, settings)
    {
    }

    public override void Enter()
    {
        slideTimer = player.SlideMaxDuration;
        slideAirTimer = 0f;
        
        slideDirection =
            Vector3.ProjectOnPlane(
                player.HorizontalVelocity,
                player.GroundNormal).normalized;
        
        player.SetCapsuleHeight(settings.crouchHeight);

        player.RigidBody.AddForce(
            slideDirection * settings.slideSpeedBoost,
            ForceMode.VelocityChange);
    }

    public override void FixedUpdate()
    {
        slideTimer -= Time.fixedDeltaTime;

        if (player.IsGrounded)
        {
            slideAirTimer = 0f;
        }
        else
        {
            slideAirTimer += Time.fixedDeltaTime;
        }

        if (slideTimer <= 0f || slideAirTimer >= player.SlideAirGracePeriod)
        {
            player.StateMachine.ChangeState(player.GroundedState);
            return;
        }
        
        Vector3 currentHorizontalVelocity = new Vector3(player.RigidBody.linearVelocity.x, 0f, player.RigidBody.linearVelocity.z);
        
        float speedToDrop = settings.slideFriction * Time.fixedDeltaTime;

        if (currentHorizontalVelocity.magnitude > speedToDrop)
        {
            player.RigidBody.linearVelocity -= currentHorizontalVelocity.normalized * speedToDrop;
        }
        
        Vector3 downhill =
            Vector3.ProjectOnPlane(Vector3.down, player.GroundNormal).normalized;

        float slopeAngle =
            Vector3.Angle(player.GroundNormal, Vector3.up);

        if (slopeAngle > 1f)
        {
            player.RigidBody.AddForce(
                downhill * settings.slideSlopeAcceleration,
                ForceMode.Acceleration);
        }
        
        Vector3 steeringInput =
            player.transform.forward * player.MoveInput.y +
            player.transform.right * player.MoveInput.x;

        if (steeringInput.sqrMagnitude > 0.001f)
        {
            steeringInput =
                Vector3.ProjectOnPlane(steeringInput, player.GroundNormal).normalized;
        
            player.RigidBody.AddForce(
                steeringInput * (settings.acceleration * 0.15f),
                ForceMode.Force);
        }
    }

    public override void Exit()
    {
        
    }
}
