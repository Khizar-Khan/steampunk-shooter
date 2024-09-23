namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementIdleState : MovementState
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
            TransitionToState(MovementStateType.CrouchState);
            return;
        }

        if (Component.IsFalling())
        {
            TransitionToState(MovementStateType.FallingState);
            return;
        }

        if (Component.CanJump())
        {
            TransitionToState(MovementStateType.JumpState);
            return;
        }

        if (Component.IsIdle())
            return;

        TransitionToState(Component.CanSprint() ? MovementStateType.SprintState : MovementStateType.WalkState);
    }
}