using Godot;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponIdleState : WeaponState
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon idle");
    }

    protected override void HandleTransitions()
    {
        if (WeaponsComponent.IsAttackRequested)
        {
            TransitionToState(WeaponStateType.AttackState);
        }
    }
}