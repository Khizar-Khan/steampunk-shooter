using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementJumpState : ComponentState<MovementComponent, MovementStates>
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
            TransitionToState(MovementStates.FallingState);
            return;
        }

        if (!Component.IsOnFloor())
            return;

        if (Component.CanSprint())
        {
            TransitionToState(MovementStates.SprintState);
            return;
        }

        if (Component.CanWalk())
        {
            TransitionToState(MovementStates.WalkState);
            return;
        }

        if (Component.IsIdle())
        {
            TransitionToState(MovementStates.IdleState);
            return;
        }
    }
}