using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementIdleState : BaseState<PlayerMovementComponent, MovementStates>
{
    public override void PhysicsProcess(double delta)
    {
        Component.Crouch((float)delta, true);

        if (!Component.IsOnFloor())
            Component.ApplyGravity(delta);

        Component.RemoveMovement(delta);
        Component.MoveAndSlide();
    }

    protected override void HandleTransitions()
    {
        if (Component.CanCrouch())
        {
            TransitionToState(MovementStates.CrouchState);
            return;
        }

        if (Component.IsFalling())
        {
            TransitionToState(MovementStates.FallingState);
            return;
        }

        if (Component.CanJump())
        {
            TransitionToState(MovementStates.JumpState);
            return;
        }

        if (Component.IsIdle())
            return;

        TransitionToState(Component.CanSprint() ? MovementStates.SprintState : MovementStates.WalkState);
    }
}