using System;
using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.systems.state_machine.states;

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