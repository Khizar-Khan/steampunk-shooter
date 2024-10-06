using System;
using Godot;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class SubState : Node
{
    protected StateMachineExtension StateMachine { get; private set; }
    protected State ParentState { get; private set; }

    public virtual void OnInitialise(StateMachineExtension stateMachineExtension, State parentState)
    {
        StateMachine = stateMachineExtension ?? throw new ArgumentNullException(nameof(stateMachineExtension));
        ParentState = parentState ?? throw new ArgumentNullException(nameof(parentState));
    }

    public virtual void Enter()
    {
        // No Default Implementation
    }

    public virtual void Exit()
    {
        // No Default Implementation
    }

    public virtual void OnProcess(double delta)
    {
        HandleSubStateTransitions();
    }

    public virtual void OnPhysicsProcess(double delta)
    {
        // No Default Implementation
    }

    protected abstract void HandleSubStateTransitions();
}