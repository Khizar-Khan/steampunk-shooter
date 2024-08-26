using System;
using Godot;
using Godot.Collections;
using SteampunkShooter.components;
using SteampunkShooter.systems.state_machine.states;

namespace SteampunkShooter.systems.state_machine;

public partial class StateMachine : Node
{
    [Export] private NodePath _initialStatePath;

    // Internal Attributes
    private Dictionary<string, State> _states;
    private State _currentState;
    public Component Component { get; private set; }

    public override async void _Ready()
    {
        Component = GetParent() as Component;
        if(Component == null)
            throw new NullReferenceException("State machine's component is null.");
        
        await ToSignal(Component, "ready");

        InitialiseStates();
        SetInitialState();
    }

    private void InitialiseStates()
    {
        _states = new Dictionary<string, State>();
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
            throw new NullReferenceException("Could not find initial state node.");

        _currentState.Enter();
    }

    public override void _Process(double delta)
    {
        _currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState.PhysicsUpdate(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _currentState.HandleInput(@event);
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