using Godot;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.weapons;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponAttackState : BaseState<PlayerWeaponsComponent, WeaponStates>
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon Attack");
        
        if (Component.CurrentWeapon.WeaponData.WeaponActivationMode == WeaponData.ActivationMode.Single)
        {
            Component.CurrentWeapon.Attack();
            Component.EmitSignal(nameof(Component.HasAttacked));
            Component.IsAttackRequested = false;
        }
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (Component.IsAttackRequested && Component.CurrentWeapon.WeaponData.WeaponActivationMode == WeaponData.ActivationMode.Continuous)
        {
            Component.CurrentWeapon.Attack();
            Component.EmitSignal(nameof(Component.HasAttacked));
        }
    }

    protected override void HandleTransitions()
    {
        if (Component.IsReloadRequested && (Component.CurrentWeapon as RangedWeapon).CanReload())
        {
            TransitionToState(WeaponStates.ReloadState);
            return;
        }
        
        if (!Component.IsAttackRequested)
        {
            TransitionToState(WeaponStates.IdleState);
        }
    }
}