using Godot;

namespace SteampunkShooter.components;

public partial class PlayerInputComponent : Component
{
    private const string MovementForward = "move_forward";
    private const string MovementBackward = "move_backward";
    private const string MovementLeft = "move_left";
    private const string MovementRight = "move_right";
    private const string MovementSprint = "sprint";
    private const string MovementJump = "jump";
    private const string MovementCrouch = "crouch";
    private const string WeaponNext = "next_weapon";
    private const string WeaponPrevious = "previous_weapon";
    private const string WeaponAttack = "attack";
    private const string WeaponReload = "reload";

    protected override void OnProcess(double delta)
    {
        Vector2 inputDirection = Input.GetVector(MovementLeft, MovementRight, MovementForward, MovementBackward);
        SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerMovementInput), inputDirection);
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEnabled)
            return;

        switch (@event)
        {
            case InputEventKey inputKeyEvent when inputKeyEvent.IsAction(MovementSprint):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerSprintRequested), inputKeyEvent.IsPressed());
                break;

            case InputEventKey inputKeyEvent when inputKeyEvent.IsActionPressed(MovementJump):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerJumpRequested));
                break;

            case InputEventKey inputKeyEvent when inputKeyEvent.IsAction(MovementCrouch):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerCrouchRequested), inputKeyEvent.IsPressed());
                break;

            case InputEventKey inputKeyEvent when inputKeyEvent.IsActionPressed(WeaponReload):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerWeaponReloadRequested));
                break;

            case InputEventMouseMotion mouseMotionEvent:
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerMouseMoved), mouseMotionEvent.Relative);
                break;

            case InputEventMouseButton mouseButtonEvent when mouseButtonEvent.IsActionPressed(WeaponNext):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerNextWeaponRequested));
                break;

            case InputEventMouseButton mouseButtonEvent when mouseButtonEvent.IsActionPressed(WeaponPrevious):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerPreviousWeaponRequested));
                break;

            case InputEventMouseButton mouseButtonEvent when mouseButtonEvent.IsAction(WeaponAttack):
                SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerWeaponAttackRequested), mouseButtonEvent.IsPressed());
                break;
        }
    }
}