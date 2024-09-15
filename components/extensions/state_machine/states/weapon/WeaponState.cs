using System;
using SteampunkShooter.systems.state_machine.states;

namespace SteampunkShooter.components.extensions.state_machine.states.weapon;

public abstract partial class WeaponState : State
{
    protected enum WeaponStateType
    {
        IdleState
    }

    protected WeaponComponent WeaponComponent { get; private set; }

    public override void Initialise(StateMachineExtension stateMachineExtension)
    {
        base.Initialise(stateMachineExtension);
        WeaponComponent = StateMachineExtension.ParentComponent as WeaponComponent;

        if (WeaponComponent == null)
            throw new NullReferenceException("StateMachine's component is not a WeaponComponent.");
    }

    protected void TransitionToState(WeaponStateType state)
    {
        StateMachineExtension.TransitionTo(state.ToString());
    }
}