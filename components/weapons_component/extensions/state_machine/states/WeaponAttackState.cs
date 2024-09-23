using Godot;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponAttackState : WeaponState
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon Attack");
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if (Component.IsAttackRequested)
        {
            Component.CurrentWeapon.Attack();
        }
    }

    protected override void HandleTransitions()
    {
        if (!Component.IsAttackRequested)
        {
            TransitionToState(WeaponStateType.IdleState);
        }
    }
}