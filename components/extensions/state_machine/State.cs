using System;
using Godot;
using Godot.Collections;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class State : Node
{
    protected StateMachineExtension StateMachine { get; private set; }

    // Internal Attributes
    private Dictionary<string, SubState> _subStates;
    private SubState _currentSubState;

    internal virtual void OnInitialise(StateMachineExtension stateMachineExtension)
    {
        StateMachine = stateMachineExtension ?? throw new ArgumentNullException(nameof(stateMachineExtension));
        _subStates = new Dictionary<string, SubState>();
        
        InitialiseSubStates();
    }
    
    private void InitialiseSubStates()
    {
        foreach (Node child in GetChildren())
        {
            if (child is not SubState subState)
                throw new InvalidCastException("Node is not a valid sub-state.");

            _subStates[child.Name] = subState;
            subState.OnInitialise(StateMachine, this);
        }
    }

    internal virtual void Enter()
    {
        // No Default Implementation
    }

    internal virtual void Exit()
    {
        _currentSubState?.Exit();
        _currentSubState = null;
    }

    internal virtual void OnProcess(double delta)
    {
        HandleStateTransitions();
        _currentSubState?.OnProcess(delta);
    }

    internal virtual void OnPhysicsProcess(double delta)
    {
        _currentSubState?.OnPhysicsProcess(delta);
    }

    public void TransitionToSubState(string key)
    {
        if (!_subStates.ContainsKey(key) || _currentSubState == _subStates[key])
        {
            GD.PrintErr($"State '{key}' does not exist or is the current sub state. Current Sub State = '{_currentSubState.Name}'.");
            return;
        }

        _currentSubState?.Exit();
        _currentSubState = _subStates[key];
        _currentSubState.Enter();
    }

    protected abstract void HandleStateTransitions();
}