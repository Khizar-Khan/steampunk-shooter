using Godot;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.weapons;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponIdleState : BaseSimpleState<PlayerWeaponsComponent, WeaponStates>
{
    internal override void Enter()
    {
        base.Enter();
    }

    protected override void HandleStateTransitions()
    {
        if (Component.IsSwitchToNextWeaponRequested || Component.IsSwitchToPreviousWeaponRequested)
        {
            TransitionToState(WeaponStates.UnequipState);
            return;
        }
        
        if (Component.IsAttackRequested)
        {
            TransitionToState(WeaponStates.AttackState);
            return;
        }
        
        if (Component.IsReloadRequested && (Component.CurrentWeapon as RangedWeapon).CanReload())
        {
            TransitionToState(WeaponStates.ReloadState);
            Component.IsReloadRequested = false;
        }
        else
        {
            Component.IsReloadRequested = false;
        }
    }
}