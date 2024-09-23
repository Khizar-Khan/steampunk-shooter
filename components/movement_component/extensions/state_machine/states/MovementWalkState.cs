namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementWalkState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        Component.SetSpeed(MovementComponent.SpeedType.Walk);
    }

    public override void PhysicsProcess(double delta)
    {
        Component.Crouch((float)delta, true);

        if (!Component.IsOnFloor())
            Component.ApplyGravity(delta);

        Component.ApplyMovement(Component.GetMovementDirectionFromInput(), delta);
        Component.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        if (Component.CanCrouch())
        {
            TransitionToState(MovementStateType.CrouchState);
            return;
        }

        if (Component.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }

        if (Component.CanJump())
        {
            TransitionToState(MovementStateType.JumpState);
            return;
        }

        if (Component.IsIdle())
        {
            TransitionToState(MovementStateType.IdleState);
            return;
        }

        if (Component.CanSprint())
        {
            TransitionToState(MovementStateType.SprintState);
            return;
        }
    }
}