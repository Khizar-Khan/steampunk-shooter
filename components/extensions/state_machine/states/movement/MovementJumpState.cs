using Godot;

namespace SteampunkShooter.systems.state_machine.states.movement;

public partial class MovementJumpState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        MovementComponent.Jump();
    }

    public override void PhysicsProcess(double delta)
    {
        MovementComponent.Crouch((float)delta, true);
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
        if (MovementComponent.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }

        if (!MovementComponent.IsOnFloor())
            return;

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