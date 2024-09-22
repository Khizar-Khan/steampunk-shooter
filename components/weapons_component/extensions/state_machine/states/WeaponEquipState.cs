using Godot;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponEquipState : WeaponState
{
    public override void Enter()
    {
        base.Enter();
        WeaponsComponent.CurrentWeapon.Show();
        GD.Print("Weapon equipped");
        
        TransitionToState(WeaponStateType.IdleState);
    }

    protected override void HandleTransitions()
    {
        // Equip state transitions automatically to Idle after entering
    }
}