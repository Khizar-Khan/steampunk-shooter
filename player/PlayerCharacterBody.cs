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
    private const string CameraComponentPath = "CameraComponent";

    [Signal]
    public delegate void ObstructionAboveEventHandler(bool isObstructionAbove);

    // References
    private ShapeCast3D _overheadShapeCast;
    private CollisionShape3D _collisionShape;
    private Node3D _cameraContainer;

    // Components
    private InputComponent _inputComponent;
    private MovementComponent _movementComponent;
    private CameraComponent _cameraComponent;

    // Cached Values
    private Transform3D _originalCameraContainerTransform;
    private float _cameraContainerPositionOffset;
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
        _cameraContainer = GetNode<Node3D>(CameraContainerPath) ?? throw new NullReferenceException("CameraContainer not found.");
    }

    private void InitialiseComponents()
    {
        _inputComponent = GetNode<InputComponent>(InputComponentPath) ?? throw new NullReferenceException("InputComponent not found.");
        _movementComponent = GetNode<MovementComponent>(MovementComponentPath) ?? throw new NullReferenceException("MovementComponent not found.");
        _cameraComponent = GetNode<CameraComponent>(CameraComponentPath) ?? throw new NullReferenceException("CameraComponent not found.");
    }

    private void CacheValues()
    {
        if (_collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            _collisionShapeStandHeight = capsuleShape.Height;
            _cameraContainerPositionOffset = _collisionShapeStandHeight - _cameraContainer.Position.Y;
        }
        else
        {
            throw new InvalidCastException("CollisionShape is not a CapsuleShape3D.");
        }
        
        _originalCameraContainerTransform = _cameraContainer.Transform;
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
        
        _inputComponent.Connect(
            InputComponent.SignalName.MouseMoved,
            new Callable(_cameraComponent, nameof(CameraComponent.OnMouseMotion))
        );
        
        _inputComponent.Connect(
            InputComponent.SignalName.MovementInput,
            new Callable(_cameraComponent, nameof(CameraComponent.OnMovementInput))
        );
    }

    private void CheckAndEmitObstructionSignal()
    {
        if (_isObstructionAbove == _overheadShapeCast.IsColliding())
            return;

        _isObstructionAbove = _overheadShapeCast.IsColliding();
        EmitSignal(SignalName.ObstructionAbove, _isObstructionAbove);
    }

    public void AdjustHeight(float targetHeight, float lerpSpeed, float threshold, float delta)
    {
        // TODO: Capsule size gets bigger when crouching and directly falling. Check via: GD.Print((_collisionShape.Shape as CapsuleShape3D).Height);
        if (_collisionShape.Shape is CapsuleShape3D capsuleShape)
        {
            float currentHeight = capsuleShape.Height;

            if (targetHeight > currentHeight && _isObstructionAbove)
                return;

            bool heightAdjusted = AdjustCapsuleHeight(capsuleShape, currentHeight, targetHeight, lerpSpeed, delta, threshold);
            bool collisionPositionAdjusted = AdjustCollisionShapePosition(targetHeight, lerpSpeed, delta, threshold);
            bool cameraPositionAdjusted = AdjustCameraContainerPosition(targetHeight, lerpSpeed, delta, threshold);

            // If all adjustments have reached the target (within the threshold), snap to the exact target values
            if (heightAdjusted && collisionPositionAdjusted && cameraPositionAdjusted)
            {
                capsuleShape.Height = targetHeight;
                _collisionShape.Position = new Vector3(
                    _collisionShape.Position.X,
                    targetHeight * 0.5f,
                    _collisionShape.Position.Z
                );
                _cameraContainer.Position = new Vector3(
                    _cameraContainer.Position.X,
                    targetHeight - _cameraContainerPositionOffset,
                    _cameraContainer.Position.Z
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

    private bool AdjustCameraContainerPosition(float targetHeight, float lerpSpeed, float delta, float threshold)
    {
        float currentCameraY = _cameraContainer.Position.Y;
        float targetCameraY = targetHeight - _cameraContainerPositionOffset;
        float newCameraY = Mathf.Lerp(currentCameraY, targetCameraY, lerpSpeed * delta);
        _cameraContainer.Position = new Vector3(
            _cameraContainer.Position.X,
            newCameraY,
            _cameraContainer.Position.Z
        );

        // Check if the camera position is within the threshold
        return Mathf.Abs(newCameraY - targetCameraY) < threshold;
    }

    public Transform3D GetOriginalCameraTransform()
    {
        return _originalCameraContainerTransform;
    }
    
    public float GetCollisionShapeStandHeight()
    {
        return _collisionShapeStandHeight;
    }
}