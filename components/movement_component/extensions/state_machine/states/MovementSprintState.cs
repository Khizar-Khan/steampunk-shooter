namespace SteampunkShooter.components.extensions.state_machine.states.movement;

public partial class MovementSprintState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        MovementComponent.SetSpeed(MovementComponent.SpeedType.Sprint);
    }

    public override void PhysicsProcess(double delta)
    {
        MovementComponent.Crouch((float)delta, true);

        if (!MovementComponent.IsOnFloor())
            MovementComponent.ApplyGravity(delta);

        MovementComponent.ApplyMovement(MovementComponent.GetMovementDirectionFromInput(), delta);
        MovementComponent.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        if (MovementComponent.CanCrouch())
        {
            TransitionToState(MovementStateType.CrouchState);
            return;
        }

        if (MovementComponent.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }

        if (MovementComponent.CanJump())
        {
            TransitionToState(MovementStateType.JumpState);
            return;
        }

        if (MovementComponent.CanSprint())
            return;

        if (MovementComponent.CanWalk())
        {
            TransitionToState(MovementStateType.WalkState);
            return;
        }

        if (MovementComponent.IsIdle())
        {
            TransitionToState(MovementStateType.IdleState);
            return;
        }
    }
}