using Godot;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.components.movement_component.extensions.state_machine.states;

namespace SteampunkShooter.components.player.player_movement_component.extensions.state_machine.states;

public partial class MovementJumpState : BaseHierarchicalState<PlayerMovementComponent, MovementStates>
{
    internal override void Enter()
    {
        base.Enter();
        Component.Jump();
    }

    internal override void OnPhysicsProcess(double delta)
    {
        Component.ApplyGravity(delta);

        Vector3 direction = Component.GetMovementDirectionFromInput();
        if (direction != Vector3.Zero)
            Component.ApplyAirMovement(direction, delta);
        else
            Component.RemoveAirMovement(delta);
        Component.MoveAndSlide();
        
        base.OnPhysicsProcess(delta);
    }

    protected override void HandleStateTransitions()
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