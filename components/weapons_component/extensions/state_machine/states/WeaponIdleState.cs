using Godot;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.weapons;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponIdleState : ComponentState<WeaponsComponent, WeaponStates>
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon idle");
    }

    protected override void HandleTransitions()
    {
        if (Component.SwitchToNextWeapon || Component.SwitchToPreviousWeapon)
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