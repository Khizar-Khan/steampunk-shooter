using System;
using Godot;
using SteampunkShooter.components;

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

    public override void Initialise(StateMachine stateMachine)
    {
        base.Initialise(stateMachine);
        MovementComponent = StateMachine.Component as MovementComponent;

        if (MovementComponent == null)
            throw new NullReferenceException("StateMachine's component is not a MovementComponent.");
    }

    protected void TransitionToState(MovementStateType state)
    {
        StateMachine.TransitionTo(state.ToString());
    }
}