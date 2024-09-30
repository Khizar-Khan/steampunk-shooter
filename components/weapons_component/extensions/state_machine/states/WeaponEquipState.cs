using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponEquipState : ComponentState<WeaponsComponent, WeaponStates>
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon equipped");
        
        if (Component.SwitchToNextWeapon)
            Component.EquipNextWeapon();
        else if (Component.SwitchToPreviousWeapon)
            Component.EquipPreviousWeapon();
        
        Component.SwitchToNextWeapon = false;
        Component.SwitchToPreviousWeapon = false;
        Component.CurrentWeapon.Show();
        
        TransitionToState(WeaponStates.IdleState);
    }

    protected override void HandleTransitions()
    {
        // Equip state transitions automatically to Idle after entering
    }
}