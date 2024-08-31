using System;
using SteampunkShooter.components;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.systems.state_machine.states.movement;

public abstract partial class MovementState : State
{
    protected enum MovementStateType
    {
        IdleState,
        WalkState,
        SprintState,
        JumpState,
        FallingState,
        CrouchState
    }

    protected MovementComponent MovementComponent { get; private set; }

    public override void Initialise(StateMachineExtension stateMachineExtension)
    {
        base.Initialise(stateMachineExtension);
        MovementComponent = StateMachineExtension.ParentComponent as MovementComponent;

        if (MovementComponent == null)
            throw new NullReferenceException("StateMachine's component is not a MovementComponent.");
    }

    protected void TransitionToState(MovementStateType state)
    {
        StateMachineExtension.TransitionTo(state.ToString());
    }
}