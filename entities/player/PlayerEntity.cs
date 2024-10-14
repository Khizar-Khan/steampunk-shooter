using System;
using Godot;
using SteampunkShooter.components;
using SteampunkShooter.components.player.player_movement_component;
using SteampunkShooter.components.weapons_component;

namespace SteampunkShooter.entities.player;

public partial class PlayerEntity : CharacterBody3D
{
    // Constant Node Paths
    private const string OverheadShapeCastPath = "OverheadShapeCast";
    private const string CollisionShapePath = "CollisionShape";
    private const string PlayerInputComponentPath = "PlayerInputComponent";
    private const string PlayerMovementComponentPath = "PlayerMovementComponent";
    private const string PlayerCameraComponentPath = "PlayerCameraComponent";
    private const string PlayerWeaponsComponentPath = "PlayerWeaponsComponent";

    // References
    private ShapeCast3D _overheadShapeCast;
    private CollisionShape3D _collisionShape;

    // Components
    private PlayerInputComponent _playerInputComponent;
    private PlayerMovementComponent _movementPlayerMovementComponent;
    private PlayerCameraComponent _playerCameraComponent;
    private PlayerWeaponsComponent _playerWeaponsComponent;

    // Cached Values
    private float _collisionShapeStandHeight;
    private bool _isObstructionAbove;

    public override void _Ready()
    {
        InitialiseReferences();
        InitialiseComponents();
        CacheValues();
        ConnectSignals();

        _overheadShapeCast.AddException(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckAndEmitObstructionSignal();
    }

    private void InitialiseReferences()
    {
        _overheadShapeCast = GetNode<ShapeCast3D>(OverheadShapeCastPath) ?? throw new NullReferenceException("OverheadShapeCast not found.");
        _collisionShape = GetNode<CollisionShape3D>(CollisionShapePath) ?? throw new NullReferenceException("CollisionShape not found.");
    }

    private void InitialiseComponents()
    {
        _playerInputComponent = GetNode<PlayerInputComponent>(PlayerInputComponentPath) ?? throw new NullReferenceException("InputComponent not found.");
        _movementPlayerMovementComponent = GetNode<PlayerMovementComponent>(PlayerMovementComponentPath) ?? throw new NullReferenceException("MovementComponent not found.");
        _playerCameraComponent = GetNode<PlayerCameraComponent>(PlayerCameraComponentPath) ?? throw new NullReferenceException("CameraComponent not found.");
        _playerWeaponsComponent = GetNode<PlayerWeaponsComponent>(PlayerWeaponsComponentPath) ?? throw new NullReferenceException("WeaponsComponent not found.");
    }

    private void CacheValues()
    {
        if (_collisionShape.Shape is CapsuleShape3D capsuleShape)
            _collisionShapeStandHeight = capsuleShape.Height;
        else
            throw new InvalidCastException("CollisionShape is not a CapsuleShape3D.");
    }

    private void ConnectSignals()
    {
        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerMovementInput),
            new Callable(_movementPlayerMovementComponent, nameof(PlayerMovementComponent.OnMovementInput))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerSprintRequested),
            new Callable(_movementPlayerMovementComponent, nameof(PlayerMovementComponent.OnSprintRequested))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerJumpRequested),
            new Callable(_movementPlayerMovementComponent, nameof(PlayerMovementComponent.OnJumpRequested))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerCrouchRequested),
            new Callable(_movementPlayerMovementComponent, nameof(PlayerMovementComponent.OnCrouchRequested))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerAdjustHeightRequested),
            new Callable(this, nameof(AdjustHeight))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerObstructionAbove),
            new Callable(_movementPlayerMovementComponent, nameof(PlayerMovementComponent.OnObstructionAbove))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerMouseMoved),
            new Callable(_playerCameraComponent, nameof(PlayerCameraComponent.OnMouseMotion))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerMovementInput),
            new Callable(_playerCameraComponent, nameof(PlayerCameraComponent.OnMovementInput))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerNextWeaponRequested),
            new Callable(_playerWeaponsComponent, nameof(PlayerWeaponsComponent.OnNextWeaponRequest))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerPreviousWeaponRequested),
            new Callable(_playerWeaponsComponent, nameof(PlayerWeaponsComponent.OnPreviousWeaponRequest))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerWeaponAttackRequested),
            new Callable(_playerWeaponsComponent, nameof(PlayerWeaponsComponent.OnWeaponAttackRequest))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerWeaponReloadRequested),
            new Callable(_playerWeaponsComponent, nameof(PlayerWeaponsComponent.OnWeaponReloadRequest))
        );

        SignalBus.Instance.Connect(
            nameof(SignalBus.Instance.PlayerMouseMoved),
            new Callable(_playerWeaponsComponent, nameof(PlayerWeaponsComponent.OnMouseMotion))
        );
    }

    private void CheckAndEmitObstructionSignal()
    {
        if (_isObstructionAbove == _overheadShapeCast.IsColliding())
            return;

        _isObstructionAbove = _overheadShapeCast.IsColliding();
        SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerObstructionAbove), _isObstructionAbove);
    }

    public void AdjustHeight(float targetHeight, float lerpSpeed, float threshold, float delta)
    {
        // TODO: Capsule size gets bigger when crouching and directly falling. Check via: GD.Print((_collisionShape.Shape as CapsuleShape3D).Height);
        if (_collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            float currentHeight = capsuleShape.Height;

            if (targetHeight > currentHeight && _isObstructionAbove)
                return;

            // TODO: Thresholds will affect different aspects differently. For example the camera position will lerp at a different rate than the capsule height.
            bool heightAdjusted = AdjustCapsuleHeight(capsuleShape, currentHeight, targetHeight, lerpSpeed, delta, threshold);
            bool collisionPositionAdjusted = AdjustCollisionShapePosition(targetHeight, lerpSpeed, delta, threshold);

            // If all adjustments have reached the target (within the threshold), snap to the exact target values
            if (heightAdjusted && collisionPositionAdjusted)
            {
                capsuleShape.Height = targetHeight;
                _collisionShape.Position = new Vector3(
                    _collisionShape.Position.X,
                    targetHeight * 0.5f,
                    _collisionShape.Position.Z
                );
            }
        }
    }

    private bool AdjustCapsuleHeight(CapsuleShape3D capsuleShape, float currentHeight, float targetHeight, float lerpSpeed, float delta, float threshold)
    {
        float newHeight = Mathf.Lerp(currentHeight, targetHeight, lerpSpeed * delta);
        capsuleShape.Height = newHeight;

        // Check if the height is within the threshold
        return Mathf.Abs(newHeight - targetHeight) < threshold;
    }

    private bool AdjustCollisionShapePosition(float targetHeight, float lerpSpeed, float delta, float threshold)
    {
        float currentCollisionY = _collisionShape.Position.Y;
        float targetCollisionY = targetHeight * 0.5f;
        float newCollisionY = Mathf.Lerp(currentCollisionY, targetCollisionY, lerpSpeed * delta);
        _collisionShape.Position = new Vector3(
            _collisionShape.Position.X,
            newCollisionY,
            _collisionShape.Position.Z
        );

        // Check if the collision shape position is within the threshold
        return Mathf.Abs(newCollisionY - targetCollisionY) < threshold;
    }

    public float GetCollisionShapeStandHeight()
    {
        return _collisionShapeStandHeight;
    }
}