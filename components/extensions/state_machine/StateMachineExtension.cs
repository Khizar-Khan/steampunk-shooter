using System;
using Godot;
using Godot.Collections;

namespace SteampunkShooter.components.extensions.state_machine;

public partial class StateMachineExtension : ComponentExtension
{
    [Export] private NodePath _initialStatePath;

    // Internal Attributes
    private Dictionary<string, State> _states;
    private State _currentState;

    internal override void OnInitialise()
    {
        base.OnInitialise();
        _states = new Dictionary<string, State>();

        InitialiseStates();
        SetInitialState();
    }

    private void InitialiseStates()
    {
        foreach (Node child in GetChildren())
        {
            if (child is not State state)
                throw new InvalidCastException("Node is not a valid state.");

            _states[child.Name] = state;
            state.Initialise(this);
        }
    }

    private void SetInitialState()
    {
        _currentState = GetNode<State>(_initialStatePath);
        if (_currentState == null)
            throw new NullReferenceException(Name + " could not find initial state node.");

        _currentState.Enter();
    }

    internal override void OnProcess(double delta)
    {
        _currentState.Process(delta);
    }

    internal override void OnPhysicsProcess(double delta)
    {
        _currentState.PhysicsProcess(delta);
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key) || _currentState == _states[key])
        {
            GD.PrintErr($"State '{key}' does not exist or is the current state. Current State = '{_currentState.Name}'.");
            return;
        }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}