using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.weapons_component.extensions.state_machine.states;

public abstract partial class WeaponState : ComponentState<WeaponsComponent, WeaponState.WeaponStateType>
{
    public enum WeaponStateType
    {
        EquipState,
        UnequipState,
        IdleState,
        AttackState,
        ReloadState
    }
}