﻿using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponEquipState : ComponentState<WeaponsComponent, WeaponStates>
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon equipped");
        
        if (Component.IsSwitchToNextWeaponRequested)
            Component.EquipNextWeapon();
        else if (Component.IsSwitchToPreviousWeaponRequested)
            Component.EquipPreviousWeapon();
        
        Component.IsSwitchToNextWeaponRequested = false;
        Component.IsSwitchToPreviousWeaponRequested = false;
        Component.CurrentWeapon.Show();
        
        TransitionToState(WeaponStates.IdleState);
    }

    protected override void HandleTransitions()
    {
        // Equip state transitions automatically to Idle after entering
    }
}