using Godot;

namespace SteampunkShooter.components.extensions.state_machine.states.movement;

public partial class MovementCrouchState : MovementState
{
    public override void Enter()
    {
        base.Enter();
        MovementComponent.SetSpeed(MovementComponent.SpeedType.Crouch);
    }

    public override void PhysicsProcess(double delta)
    {
        MovementComponent.Crouch((float)delta);

        if (!MovementComponent.IsOnFloor())
            MovementComponent.ApplyGravity(delta);

        MovementComponent.ApplyMovement(MovementComponent.GetMovementDirectionFromInput(), delta);
        MovementComponent.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        // Check if the player is no longer requesting to crouch (e.g., crouch key is released)
        if (!MovementComponent.CanCrouch() && MovementComponent.CanStand())
        {
            if (MovementComponent.IsIdle())
            {
                TransitionToState(MovementStateType.IdleState);
                return;
            }

            if (MovementComponent.GetMovementDirectionFromInput() != Vector3.Zero)
            {
                if (MovementComponent.CanSprint())
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
            if (MovementComponent.CanJump())
            {
                TransitionToState(MovementStateType.JumpState);
                return;
            }
        }

        // Check if the player is falling (e.g., walked off a ledge or was pushed)
        if (MovementComponent.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }
    }
}