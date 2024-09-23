using Godot;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementJumpState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        Component.Jump();
    }

    public override void PhysicsProcess(double delta)
    {
        Component.Crouch((float)delta, true);
        Component.ApplyGravity(delta);

        Vector3 direction = Component.GetMovementDirectionFromInput();
        if (direction != Vector3.Zero)
            Component.ApplyAirMovement(direction, delta);
        else
            Component.RemoveAirMovement(delta);
        Component.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        if (Component.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }

        if (!Component.IsOnFloor())
            return;

        if (Component.CanSprint())
        {
            TransitionToState(MovementStateType.SprintState);
            return;
        }

        if (Component.CanWalk())
        {
            TransitionToState(MovementStateType.WalkState);
            return;
        }

        if (Component.IsIdle())
        {
            TransitionToState(MovementStateType.IdleState);
            return;
        }
    }
}