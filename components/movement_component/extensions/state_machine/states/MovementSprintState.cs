using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public partial class MovementSprintState : ComponentState<MovementComponent, MovementStates>
{
    public override void Enter()
    {
        base.Enter();
        Component.SetSpeed(MovementComponent.SpeedType.Sprint);
    }

    public override void PhysicsProcess(double delta)
    {
        Component.Crouch((float)delta, true);

        if (!Component.IsOnFloor())
            Component.ApplyGravity(delta);

        Component.ApplyMovement(Component.GetMovementDirectionFromInput(), delta);
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

        if (Component.CanSprint())
            return;

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