namespace SteampunkShooter.components.extensions.state_machine.states.movement;

public partial class MovementWalkState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        MovementComponent.SetSpeed(MovementComponent.SpeedType.Walk);
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

        if (MovementComponent.IsIdle())
        {
            TransitionToState(MovementStateType.IdleState);
            return;
        }

        if (MovementComponent.CanSprint())
        {
            TransitionToState(MovementStateType.SprintState);
            return;
        }
    }
}