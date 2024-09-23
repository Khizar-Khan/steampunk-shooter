using System;

namespace SteampunkShooter.components.extensions.state_machine;

public abstract partial class ComponentState<T, TE> : State where T : Component where TE : Enum
{
    protected T Component { get; private set; }

    public override void Initialise(StateMachineExtension stateMachineExtension)
    {
        base.Initialise(stateMachineExtension);
        Component = StateMachineExtension.ParentComponent as T;

        if (Component == null)
            throw new NullReferenceException($"StateMachine's component is not of type {typeof(T).Name}.");
    }

    protected void TransitionToState(TE stateEnum)
    {
        StateMachineExtension.TransitionTo(stateEnum.ToString());
    }
}