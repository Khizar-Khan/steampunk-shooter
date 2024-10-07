using Godot;
using SteampunkShooter.components.movement_component.extensions.state_machine.states;

namespace SteampunkShooter.components.extensions.state_machine.sub_states;

public partial class MovementIdleSubState : SubState<PlayerMovementComponent, MovementStates>
{
    internal override void OnProcess(double delta)
    {
        base.OnProcess(delta);
        HandleSubStateTransitions();
    }
    
    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        GD.Print("PhysicsProcess");
    }

    protected override void HandleSubStateTransitions()
    {
        if (ParentState.Component.CanCrouchInAir())
        {
            TransitionToSubState(MovementStates.CrouchState);
        }
    }
}