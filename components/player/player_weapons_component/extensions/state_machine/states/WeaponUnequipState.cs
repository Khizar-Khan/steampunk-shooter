using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponUnequipState : BaseSimpleState<PlayerWeaponsComponent, WeaponStates>
{
    internal override void Enter()
    {
        base.Enter();
        Component.CurrentWeapon.Hide();
        GD.Print("Weapon Unequipped");

        TransitionToState(WeaponStates.EquipState);
    }
    
    protected override void HandleStateTransitions()
    {
        // EMPTY
    }
}