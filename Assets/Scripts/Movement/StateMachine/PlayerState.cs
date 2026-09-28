

public abstract class PlayerState
{
    protected PlayerMovement player;
    protected MovementSettings settings;

    public PlayerState(PlayerMovement player, MovementSettings settings)
    {
        this.player = player;
        this.settings = settings;
    }

    public virtual void Enter() {}
    public virtual void Exit() {}
    
    public virtual void Update() {}
    public virtual void FixedUpdate() {}
}
