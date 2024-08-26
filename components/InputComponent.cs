using Godot;

namespace SteampunkShooter.components;

public partial class InputComponent : Component
{
    private const string MovementForward = "move_forward";
    private const string MovementBackward = "move_backward";
    private const string MovementLeft = "move_left";
    private const string MovementRight = "move_right";
    private const string MovementSprint = "sprint";
    private const string MovementJump = "jump";
    private const string MovementCrouch = "crouch";

    [Signal]
    public delegate void MovementInputEventHandler(Vector2 inputDirection);

    [Signal]
    public delegate void SprintRequestedEventHandler(bool isRequested);

    [Signal]
    public delegate void JumpRequestedEventHandler();

    [Signal]
    public delegate void CrouchRequestedEventHandler(bool isRequested);

    [Signal]
    public delegate void MouseMovedEventHandler(Vector2 mouseDelta);

    protected override void ProcessComponent(double delta)
    {
        Vector2 inputDirection = Input.GetVector(MovementLeft, MovementRight, MovementForward, MovementBackward);
        EmitSignal(SignalName.MovementInput, inputDirection);
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEnabled)
            return;

        switch (@event)
        {
            case InputEventKey inputKeyEvent when inputKeyEvent.IsAction(MovementSprint):
                EmitSignal(SignalName.SprintRequested, inputKeyEvent.IsPressed());
                break;

            case InputEventKey inputKeyEvent when inputKeyEvent.IsActionPressed(MovementJump):
                EmitSignal(SignalName.JumpRequested);
                break;

            case InputEventKey inputKeyEvent when inputKeyEvent.IsAction(MovementCrouch):
                EmitSignal(SignalName.CrouchRequested, inputKeyEvent.IsPressed());
                break;

            case InputEventMouseMotion mouseMotionEvent:
                EmitSignal(SignalName.MouseMoved, mouseMotionEvent.Relative);
                break;
        }
    }
}