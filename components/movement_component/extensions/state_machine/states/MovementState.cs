using SteampunkShooter.components.extensions.state_machine;

namespace SteampunkShooter.components.movement_component.extensions.state_machine.states;

public abstract partial class MovementState : ComponentState<MovementComponent, MovementState.MovementStateType>
{
    public enum MovementStateType
    {
        IdleState,
        WalkState,
        SprintState,
        JumpState,
        FallingState,
        CrouchState
    }
}