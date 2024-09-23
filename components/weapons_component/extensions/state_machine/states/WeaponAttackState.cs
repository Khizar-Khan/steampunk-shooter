using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponAttackState : ComponentState<WeaponsComponent, WeaponStates>
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
            TransitionToState(WeaponStates.IdleState);
        }
    }
}