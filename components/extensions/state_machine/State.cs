using System;
using Godot;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class State : Node
{
    protected StateMachineExtension StateMachine { get; private set; }

    internal virtual void OnInitialise(StateMachineExtension stateMachineExtension)
    {
        StateMachine = stateMachineExtension ?? throw new ArgumentNullException(nameof(stateMachineExtension));
    }

    internal virtual void Enter()
    {
        // No Default Implementation
    }

    internal virtual void Exit()
    {
        // No Default Implementation
    }

    internal virtual void OnProcess(double delta)
    {
        HandleStateTransitions();
    }

    internal virtual void OnPhysicsProcess(double delta)
    {
        // No Default Implementation
    }

    protected abstract void HandleStateTransitions();
}