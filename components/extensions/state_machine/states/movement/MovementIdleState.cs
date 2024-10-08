using Godot;

namespace SteampunkShooter.systems.state_machine.states.movement;

public partial class MovementIdleState : MovementState
{
    public override void PhysicsProcess(double delta)
    {
        MovementComponent.Crouch((float)delta, true);
        
        if (!MovementComponent.IsOnFloor())
            MovementComponent.ApplyGravity(delta);

        MovementComponent.RemoveMovement(delta);
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
            return;

        TransitionToState(MovementComponent.CanSprint() ? MovementStateType.SprintState : MovementStateType.WalkState);
    }
}