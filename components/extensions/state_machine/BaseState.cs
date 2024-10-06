using System;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class BaseState<T, TE> : State where T : Component where TE : Enum
{
    protected T Component { get; private set; }

    internal override void OnInitialise(StateMachineExtension stateMachineExtension)
    {
        base.OnInitialise(stateMachineExtension);
        Component = StateMachine.ParentComponent as T;

        if (Component == null)
            throw new NullReferenceException($"StateMachine's component is not of type {typeof(T).Name}.");
    }

    protected void TransitionToState(TE stateEnum)
    {
        StateMachine.TransitionToState(stateEnum.ToString());
    }
}