using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponEquipState : ComponentState<WeaponsComponent, WeaponStates>
{
    public override void Enter()
    {
        base.Enter();
        Component.CurrentWeapon.Show();
        GD.Print("Weapon equipped");

        TransitionToState(WeaponStates.IdleState);
    }

    protected override void HandleTransitions()
    {
        // Equip state transitions automatically to Idle after entering
    }
}