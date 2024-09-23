using Godot;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementCrouchState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        Component.SetSpeed(MovementComponent.SpeedType.Crouch);
    }

    public override void PhysicsProcess(double delta)
    {
        Component.Crouch((float)delta);

        if (!Component.IsOnFloor())
            Component.ApplyGravity(delta);

        Component.ApplyMovement(Component.GetMovementDirectionFromInput(), delta);
        Component.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        // Check if the player is no longer requesting to crouch (e.g., crouch key is released)
        if (!Component.CanCrouch() && Component.CanStand())
        {
            if (Component.IsIdle())
            {
                TransitionToState(MovementStateType.IdleState);
                return;
            }

            if (Component.GetMovementDirectionFromInput() != Vector3.Zero)
            {
                if (Component.CanSprint())
                {
                    TransitionToState(MovementStateType.SprintState);
                    return;
                }
                else
                {
                    TransitionToState(MovementStateType.WalkState);
                    return;
                }
            }

            // Check if the player is trying to jump while crouched
            if (Component.CanJump())
            {
                TransitionToState(MovementStateType.JumpState);
                return;
            }
        }

        // Check if the player is falling (e.g., walked off a ledge or was pushed)
        if (Component.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }
    }
}