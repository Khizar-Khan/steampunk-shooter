using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.components.movement_component.extensions.state_machine.states;

namespace SteampunkShooter.components.player.player_movement_component.extensions.state_machine.states;

public partial class MovementSprintState : BaseSimpleState<PlayerMovementComponent, MovementStates>
{
    internal override void Enter()
    {
        base.Enter();
        Component.SetSpeed(PlayerMovementComponent.SpeedType.Sprint);
    }

    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        
        Component.Crouch((float)delta, true);

        if (!Component.IsOnFloor())
            Component.ApplyGravity(delta);

        Component.ApplyMovement(Component.GetMovementDirectionFromInput(), delta);
        Component.MoveAndSlide();
    }

    protected override void HandleStateTransitions()
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