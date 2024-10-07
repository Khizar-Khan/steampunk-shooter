using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementCrouchState : BaseSimpleState<PlayerMovementComponent, MovementStates>
{
    internal override void Enter()
    {
        base.Enter();
        Component.SetSpeed(PlayerMovementComponent.SpeedType.Crouch);
    }

    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        
        Component.Crouch((float)delta);

        if (!Component.IsOnFloor())
            Component.ApplyGravity(delta);

        Component.ApplyMovement(Component.GetMovementDirectionFromInput(), delta);
        Component.MoveAndSlide();
    }

    protected override void HandleStateTransitions()
    {
        // Check if the player is no longer requesting to crouch (e.g., crouch key is released)
        if (!Component.CanCrouch() && Component.CanStand())
        {
            if (Component.IsIdle())
            {
                TransitionToState(MovementStates.IdleState);
                return;
            }

            if (Component.GetMovementDirectionFromInput() != Vector3.Zero)
            {
                if (Component.CanSprint())
                {
                    TransitionToState(MovementStates.SprintState);
                    return;
                }
                else
                {
                    TransitionToState(MovementStates.WalkState);
                    return;
                }
            }

            // Check if the player is trying to jump while crouched
            if (Component.CanJump())
            {
                TransitionToState(MovementStates.JumpState);
                return;
            }
        }

        // Check if the player is falling (e.g., walked off a ledge or was pushed)
        if (Component.IsFalling())
        {
            TransitionToState(MovementStates.FallingState);
            return;
        }
    }
}