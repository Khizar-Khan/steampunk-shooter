using System;
using SteampunkShooter.components.extensions.state_machine.states;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class ComponentState<T> : State where T : Component
{
    protected T Component { get; private set; }

    public override void Initialise(StateMachineExtension stateMachineExtension)
    {
        base.Initialise(stateMachineExtension);
        Component = StateMachineExtension.ParentComponent as T;

        if (Component == null)
            throw new NullReferenceException($"StateMachine's component is not of type {typeof(T).Name}.");
    }

    protected void TransitionToState<TE>(TE stateEnum) where TE : Enum
    {
        StateMachineExtension.TransitionTo(stateEnum.ToString());
    }
}