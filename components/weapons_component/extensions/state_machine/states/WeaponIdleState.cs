using Godot;
using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public partial class WeaponIdleState : ComponentState<WeaponsComponent, WeaponStates>
{
    public override void Enter()
    {
        base.Enter();
        GD.Print("Weapon idle");
    }

    protected override void HandleTransitions()
    {
        if (Component.IsAttackRequested)
        {
            TransitionToState(WeaponStates.AttackState);
        }
    }
}