using Godot;
using SteampunkShooter.components.movement_component.extensions.state_machine.states;

namespace SteampunkShooter.components.extensions.state_machine.sub_states;

public partial class MovementCrouchSubState : SubState<PlayerMovementComponent, MovementStates>
{
    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        ParentState.Component.Crouch((float)delta);
    }

    protected override void HandleSubStateTransitions()
    {
        if (!ParentState.Component.CanCrouchInAir())
        {
            TransitionToSubState(MovementStates.IdleState);
        }
    }
}