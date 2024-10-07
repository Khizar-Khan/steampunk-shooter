using System;
using Godot;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class SubState<T, TE> : Node where T : Component where TE : Enum
{
    protected BaseHierarchicalState<T, TE> ParentState { get; private set; }

    public virtual void OnInitialise(BaseHierarchicalState<T, TE> parentHierarchicalState)
    {
        ParentState = parentHierarchicalState ?? throw new ArgumentNullException(nameof(parentHierarchicalState));
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
        HandleSubStateTransitions();
    }

    internal virtual void OnPhysicsProcess(double delta)
    {
        // No Default Implementation
    }
    
    protected void TransitionToSubState(TE stateEnum)
    {
        ParentState.TransitionToSubState(stateEnum.ToString());
    }

    protected abstract void HandleSubStateTransitions();
}