using System;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.components.extensions.state_machine.states;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public abstract partial class WeaponState : State
{
    protected enum WeaponStateType
    {
        EquipState,
        UnEquipState,
        IdleState,
        AttackState,
        ReloadState
    }
    
    protected WeaponsComponent WeaponsComponent { get; private set; }
    
    public override void Initialise(StateMachineExtension stateMachineExtension)
    {
        base.Initialise(stateMachineExtension);
        WeaponsComponent = StateMachineExtension.ParentComponent as WeaponsComponent;

        if (WeaponsComponent == null)
            throw new NullReferenceException("StateMachine's component is not a WeaponComponent.");
    }
    
    protected void TransitionToState(WeaponStateType state)
    {
        StateMachineExtension.TransitionTo(state.ToString());
    }
}