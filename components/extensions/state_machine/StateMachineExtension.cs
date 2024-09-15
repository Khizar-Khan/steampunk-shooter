using System;
using Godot;
using Godot.Collections;
using SteampunkShooter.systems.state_machine.states;

namespace SteampunkShooter.components.extensions;

public partial class StateMachineExtension : ComponentExtension
{
    [Export] private NodePath _initialStatePath;

    // Internal Attributes
    private Dictionary<string, State> _states;
    private State _currentState;

    public override void Initialise()
    {
        base.Initialise();
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

    public override void Process(double delta)
    {
        _currentState.Process(delta);
    }

    public override void PhysicsProcess(double delta)
    {
        _currentState.PhysicsProcess(delta);
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key) || _currentState == _states[key])
            throw new NullReferenceException("State does not exist.");

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}