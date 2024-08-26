using System;
using Godot;

namespace SteampunkShooter.systems.state_machine.states;

public abstract partial class State : Node
{
    protected StateMachine StateMachine { get; private set; }

    public virtual void Initialise(StateMachine stateMachine)
    {
        StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
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