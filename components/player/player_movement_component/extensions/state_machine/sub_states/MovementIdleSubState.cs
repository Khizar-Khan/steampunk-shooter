using SteampunkShooter.components.movement_component.extensions.state_machine.states;

namespace SteampunkShooter.components.extensions.state_machine.sub_states;

public partial class MovementIdleSubState : SubState<PlayerMovementComponent, MovementStates>
{
    internal override void OnProcess(double delta)
    {
        ParentState.Component.Crouch((float)delta, true);
        base.OnProcess(delta);
    }

    protected override void HandleSubStateTransitions()
    {
        if (ParentState.Component.CanCrouchInAir())
        {
            TransitionToSubState(MovementStates.CrouchState);
        }
    }
}