using Godot;

namespace SteampunkShooter.components.extensions.state_machine.states.movement;

public partial class MovementFallingState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        MovementComponent.StartCoyoteTimer();
    }

    public override void Exit()
    {
        base.Exit();
        MovementComponent.StopCoyoteTimer();
        MovementComponent.ResetLandingFlags();
    }

    public override void PhysicsProcess(double delta)
    {
        MovementComponent.ApplyGravity(delta);

        Vector3 direction = MovementComponent.GetMovementDirectionFromInput();
        if (direction != Vector3.Zero)
            MovementComponent.ApplyAirMovement(direction, delta);
        else
            MovementComponent.RemoveAirMovement(delta);
        MovementComponent.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        if (MovementComponent.CanJump())
        {
            TransitionToState(MovementStateType.JumpState);
            return;
        }

        if (!MovementComponent.IsOnFloor())
            return;

        if (MovementComponent.CanCrouch())
        {
            TransitionToState(MovementStateType.CrouchState);
            return;
        }

        if (MovementComponent.CanSprint())
        {
            TransitionToState(MovementStateType.SprintState);
            return;
        }

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