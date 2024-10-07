using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementFallingState : BaseState<PlayerMovementComponent, MovementStates>
{
    internal override void Enter()
    {
        base.Enter();
        Component.StartCoyoteTimer();
    }

    internal override void Exit()
    {
        base.Exit();
        Component.StopCoyoteTimer();
        Component.ResetLandingFlags();
    }

    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        
        Component.ApplyGravity(delta);

        Vector3 direction = Component.GetMovementDirectionFromInput();
        if (direction != Vector3.Zero)
            Component.ApplyAirMovement(direction, delta);
        else
            Component.RemoveAirMovement(delta);
        Component.MoveAndSlide();
    }

    protected override void HandleStateTransitions()
    {
        if (Component.CanJump())
        {
            TransitionToState(MovementStates.JumpState);
            return;
        }

        if (!Component.IsOnFloor())
            return;

        if (Component.CanCrouch())
        {
            TransitionToState(MovementStates.CrouchState);
            return;
        }

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