using Godot;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }
    
    # region Player Signals
    [Signal]
    public delegate void PlayerMovementInputEventHandler(Vector2 inputDirection);

    [Signal]
    public delegate void PlayerSprintRequestedEventHandler(bool isRequested);

    [Signal]
    public delegate void PlayerJumpRequestedEventHandler();

    [Signal]
    public delegate void PlayerCrouchRequestedEventHandler(bool isRequested);

    [Signal]
    public delegate void PlayerMouseMovedEventHandler(Vector2 mouseDelta);

    [Signal]
    public delegate void PlayerNextWeaponRequestedEventHandler();

    [Signal]
    public delegate void PlayerPreviousWeaponRequestedEventHandler();

    [Signal]
    public delegate void PlayerWeaponAttackRequestedEventHandler(bool isRequested);

    [Signal]
    public delegate void PlayerWeaponReloadRequestedEventHandler();
    
    [Signal]
    public delegate void PlayerObstructionAboveEventHandler(bool isObstructionAbove);
    
    [Signal]
    public delegate void PlayerAdjustHeightRequestedEventHandler(float targetHeight, float lerpSpeed, float delta);
    
    [Signal]
    public delegate void PlayerHasAttackedEventHandler();
    # endregion
    
    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            GD.PrintErr("Warning: More than one instance of EventBus exists. This should be avoided.");
    }
}