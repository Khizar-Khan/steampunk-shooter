using Godot;
using SteampunkShooter.components.extensions.state_machine;
using SteampunkShooter.weapons;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponAttackState : BaseSimpleState<PlayerWeaponsComponent, WeaponStates>
{
    internal override void Enter()
    {
        base.Enter();
        GD.Print("Weapon Attack");
        
        if (Component.CurrentWeapon.WeaponData.WeaponActivationMode == WeaponData.ActivationMode.Single)
        {
            Component.CurrentWeapon.Attack();
            SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerHasAttacked));
            Component.IsAttackRequested = false;
        }
    }

    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);

        if (Component.IsAttackRequested && Component.CurrentWeapon.WeaponData.WeaponActivationMode == WeaponData.ActivationMode.Continuous)
        {
            Component.CurrentWeapon.Attack();
            SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerHasAttacked));
        }
    }

    protected override void HandleStateTransitions()
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