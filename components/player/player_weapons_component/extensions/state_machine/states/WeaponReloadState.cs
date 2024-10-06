using Godot;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.weapons;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponReloadState : BaseState<PlayerWeaponsComponent, WeaponStates>
{
    private RangedWeapon _rangedWeapon;
    
    internal override void Enter()
    {
        base.Enter();
        GD.Print("Entering Reload State");

        if (Component.CurrentWeapon is RangedWeapon rangedWeapon)
        {
            _rangedWeapon = rangedWeapon;
            _rangedWeapon.Reload();
        }
    }

    protected override void HandleStateTransitions()
    {
        if (_rangedWeapon.ReloadTimer.IsStopped())
        {
            TransitionToState(WeaponStates.IdleState);
        }
    }
}