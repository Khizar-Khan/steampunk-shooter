using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class BaseState<T, TE> : State where T : Component where TE : Enum
{
    [Export] private NodePath _initialSubStatePath;

    public T Component { get; private set; }

    // Internal Attributes
    private Dictionary<string, SubState<T, TE>> _subStates;
    private SubState<T, TE> _currentSubState;

    internal override void OnInitialise(StateMachineExtension stateMachineExtension)
    {
        base.OnInitialise(stateMachineExtension);
        Component = StateMachine.ParentComponent as T ?? throw new NullReferenceException($"StateMachine's component is not of type {typeof(T).Name}.");
        _subStates = new Dictionary<string, SubState<T, TE>>();
        InitialiseSubStates();
    }

    internal override void Enter()
    {
        base.Enter();
        SetInitialSubState();
    }

    internal override void Exit()
    {
        base.Exit();
        _currentSubState?.Exit();
        _currentSubState = null;
    }

    internal override void OnProcess(double delta)
    {
        base.OnProcess(delta);
        _currentSubState?.OnProcess(delta);
    }

    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        _currentSubState?.OnPhysicsProcess(delta);
    }

    public void TransitionToSubState(string key)
    {
        if (_currentSubState == _subStates[key])
            return;

        if (!_subStates.ContainsKey(key))
        {
            GD.PrintErr($"State '{key}' does not exist. Current Sub State = '{_currentSubState.Name}'.");
            return;
        }

        _currentSubState?.Exit();
        _currentSubState = _subStates[key];
        _currentSubState.Enter();
    }

    protected void TransitionToState(TE stateEnum)
    {
        StateMachine.TransitionToState(stateEnum.ToString());
    }

    private void InitialiseSubStates()
    {
        foreach (SubState<T, TE> subState in GetChildren().OfType<SubState<T, TE>>())
        {
            _subStates[subState.Name] = subState;
            subState.OnInitialise(this);
        }
    }

    private void SetInitialSubState()
    {
        if (_subStates.Count == 0)
            return;

        GD.Print($"{Name}: Initial Sub State Path: {_initialSubStatePath}");
        _currentSubState = GetNode<SubState<T, TE>>(_initialSubStatePath);

        if (_currentSubState == null)
            throw new NullReferenceException("Initial Sub State Not Found");

        _currentSubState.Enter();
    }
}