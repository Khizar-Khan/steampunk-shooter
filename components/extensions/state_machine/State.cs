using System;
using Godot;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class State : Node
{
    protected StateMachineExtension StateMachineExtension { get; private set; }

    public virtual void Initialise(StateMachineExtension stateMachineExtension)
    {
        StateMachineExtension = stateMachineExtension ?? throw new ArgumentNullException(nameof(stateMachineExtension));
    }

    public virtual void Enter()
    {
        // No Default Implementation
    }

    public virtual void Exit()
    {
        // No Default Implementation
    }

    public virtual void Process(double delta)
    {
        HandleTransitions();
    }

    public virtual void PhysicsProcess(double delta)
    {
        // No Default Implementation
    }

    protected abstract void HandleTransitions();
}