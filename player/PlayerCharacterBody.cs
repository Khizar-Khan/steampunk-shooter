using System;
using Godot;
using SteampunkShooter.components;

namespace SteampunkShooter.player;

public partial class PlayerCharacterBody : CharacterBody3D
{
    // Constant Node Paths
    private const string OverheadShapeCastPath = "OverheadShapeCast";
    private const string CollisionShapePath = "CollisionShape";
    private const string CameraContainerPath = "CameraContainer";
    private const string InputComponentPath = "InputComponent";
    private const string MovementComponentPath = "MovementComponent";

    [Signal]
    public delegate void ObstructionAboveEventHandler(bool isObstructionAbove);
    
    // References
    private ShapeCast3D _overheadShapeCast;
    private CollisionShape3D _collisionShape;
    private Node3D _cameraContainer;

    // Components
    private InputComponent _inputComponent;
    private MovementComponent _movementComponent;

    // Caching Values
    private float _cameraContainerPositionOffset;
    private bool _isObstructionAbove;
    public float CollisionShapeStandHeight { get; private set; }

    public override void _Ready()
    {
        InitializeReferences();
        InitializeComponents();
        CacheValues();
        ConnectSignals();
        
        _overheadShapeCast.AddException(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckAndEmitObstructionSignal();
    }

    private void InitializeReferences()
    {
        _overheadShapeCast = GetNode<ShapeCast3D>(OverheadShapeCastPath) ?? throw new NullReferenceException("OverheadShapeCast not found.");
        _collisionShape = GetNode<CollisionShape3D>(CollisionShapePath) ?? throw new NullReferenceException("CollisionShape not found.");
        _cameraContainer = GetNode<Node3D>(CameraContainerPath) ?? throw new NullReferenceException("CameraContainer not found.");
    }

    private void InitializeComponents()
    {
        _inputComponent = GetNode<InputComponent>(InputComponentPath) ?? throw new NullReferenceException("InputComponent not found.");
        _movementComponent = GetNode<MovementComponent>(MovementComponentPath) ?? throw new NullReferenceException("MovementComponent not found.");
    }

    private void CacheValues()
    {
        if (_collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            CollisionShapeStandHeight = capsuleShape.Height;
            _cameraContainerPositionOffset = CollisionShapeStandHeight - _cameraContainer.Position.Y;
        }
        else
        {
            throw new InvalidCastException("CollisionShape is not a CapsuleShape3D.");
        }
    }

    private void ConnectSignals()
    {
        _inputComponent.Connect(
            nameof(InputComponent.MovementInput),
            new Callable(_movementComponent, nameof(MovementComponent.OnMovementInput))
        );

        _inputComponent.Connect(
            nameof(InputComponent.SprintRequested),
            new Callable(_movementComponent, nameof(MovementComponent.OnSprintRequested))
        );

        _inputComponent.Connect(
            nameof(InputComponent.JumpRequested),
            new Callable(_movementComponent, nameof(MovementComponent.OnJumpRequested))
        );

        _inputComponent.Connect(
            nameof(InputComponent.CrouchRequested),
            new Callable(_movementComponent, nameof(MovementComponent.OnCrouchRequested))
        );

        _movementComponent.Connect(
            nameof(MovementComponent.AdjustHeightRequested),
            new Callable(this, nameof(AdjustHeight))
        );

        Connect(
            nameof(ObstructionAbove),
            new Callable(_movementComponent, nameof(MovementComponent.OnObstructionAbove))
        );
    }

    private void CheckAndEmitObstructionSignal()
    {
        if (_isObstructionAbove == _overheadShapeCast.IsColliding())
            return;

        _isObstructionAbove = _overheadShapeCast.IsColliding();
        EmitSignal(SignalName.ObstructionAbove, _isObstructionAbove);
    }

    public void AdjustHeight(float targetHeight, float lerpSpeed, float delta)
    {
        if (_collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            float currentHeight = capsuleShape.Height;
            if (targetHeight > currentHeight && _isObstructionAbove)
                return;

            AdjustCapsuleHeight(capsuleShape, currentHeight, targetHeight, lerpSpeed, delta);
            AdjustCollisionShapePosition(targetHeight, lerpSpeed, delta);
            AdjustCameraContainerPosition(targetHeight, lerpSpeed, delta);
        }
    }

    private void AdjustCapsuleHeight(CapsuleShape3D capsuleShape, float currentHeight, float targetHeight, float lerpSpeed, float delta)
    {
        float newHeight = Mathf.Lerp(currentHeight, targetHeight, lerpSpeed * delta);
        capsuleShape.Height = newHeight;
    }

    private void AdjustCollisionShapePosition(float targetHeight, float lerpSpeed, float delta)
    {
        float currentCollisionY = _collisionShape.Position.Y;
        float targetCollisionY = targetHeight * 0.5f;
        float newCollisionY = Mathf.Lerp(currentCollisionY, targetCollisionY, lerpSpeed * delta);
        _collisionShape.Position = new Vector3(
            _collisionShape.Position.X,
            newCollisionY,
            _collisionShape.Position.Z
        );
    }

    private void AdjustCameraContainerPosition(float targetHeight, float lerpSpeed, float delta)
    {
        float currentCameraY = _cameraContainer.Position.Y;
        float targetCameraY = targetHeight - _cameraContainerPositionOffset;
        float newCameraY = Mathf.Lerp(currentCameraY, targetCameraY, lerpSpeed * delta);
        _cameraContainer.Position = new Vector3(
            _cameraContainer.Position.X,
            newCameraY,
            _cameraContainer.Position.Z
        );
    }
}