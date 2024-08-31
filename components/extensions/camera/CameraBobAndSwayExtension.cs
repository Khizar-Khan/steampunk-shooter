using System;
using Godot;
using SteampunkShooter.player;

namespace SteampunkShooter.components.extensions.camera;

public partial class CameraBobAndSwayExtension : ComponentExtension
{
    // TODO: Probably needs a rework...
    
    // Constants
    private const float MovementThreshold = 0.05f;
    private const float MaxPlayerSpeed = 4.5f; // Max player speed for speed factor calculation
    private const float MidAirBobReductionFactor = 0.5f; // Reduce bobbing intensity mid-air
    
    [Export] private float _bobFrequency = 3.25f;
    [Export] private float _bobAmplitude = 0.06f;
    [Export] private float _swayFrequency = 2.0f;
    [Export] private float _swayAmplitude = 0.03f;
    [Export] private float _swayRotationLimit = 1.0f;
    [Export] private float _cameraPositionResetSpeed = 5.0f;
    [Export] private float _cameraRotationResetSpeed = 5.0f;
    
    // References
    private CameraComponent _cameraComponent;
    private Transform3D _originalCameraContainerTransform;
    
    // Cached Values
    private double _bobAndSwayTime;
    private float _swayDirection;
    private Vector3 _lastBobAndSwayOffset;

    public override void Initialise()
    {
        base.Initialise();

        _cameraComponent = ParentComponent as CameraComponent;
        if (_cameraComponent == null)
        {
            GD.PrintErr("ParentComponent is not a CameraComponent!");
            return;
        }

        PlayerCharacterBody playerCharacterBody = (_cameraComponent.CharacterBody as PlayerCharacterBody);
        if (playerCharacterBody == null)
            throw new NullReferenceException("Player Character Body not found in Camera Bob and Sway Extension.");
        
        _originalCameraContainerTransform = playerCharacterBody.GetOriginalCameraTransform();
        _bobAndSwayTime = 0.0f;
        _lastBobAndSwayOffset = Vector3.Zero;
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        HandleBobAndSway(delta);
    }

    // Handles bob and sway based on player movement and state (e.g., mid-air or on ground)
    private void HandleBobAndSway(double delta)
    {
        Vector3 velocity = GetHorizontalVelocity();
        float movementSpeed = velocity.Length();

        bool isOnFloor = _cameraComponent.CharacterBody.IsOnFloor();
        bool isMoving = movementSpeed > MovementThreshold;

        UpdateBobAndSwayTime(movementSpeed, delta, isOnFloor);

        Vector3 newBobAndSwayOffset = Vector3.Zero;
        if (isMoving || _bobAndSwayTime > 0.0f)
        {
            newBobAndSwayOffset = CalculateBobAndSwayOffset(velocity, delta, isOnFloor);
        }

        ApplyBobAndSwayOffset(newBobAndSwayOffset);
        HandleRotationalSway(delta);
    }

    // Returns the horizontal velocity, excluding vertical movement
    private Vector3 GetHorizontalVelocity()
    {
        Vector3 velocity = _cameraComponent.CharacterBody.Velocity;
        velocity.Y = 0; // Ignore vertical movement
        return velocity;
    }

    // Updates the bob and sway time based on movement speed and whether the player is on the ground
    private void UpdateBobAndSwayTime(float movementSpeed, double delta, bool isOnFloor)
    {
        if (movementSpeed > MovementThreshold && isOnFloor)
        {
            _bobAndSwayTime += (float)(delta * movementSpeed);
        }
        else
        {
            // Smoothly reduce the bob and sway time if not moving or mid-air
            _bobAndSwayTime = Mathf.Max(_bobAndSwayTime - (float)(delta * _cameraPositionResetSpeed), 0.0f);
        }
    }

    // Calculates the bob and sway offset based on movement, delta, and player state
    private Vector3 CalculateBobAndSwayOffset(Vector3 velocity, double delta, bool isOnFloor)
    {
        float speedFactor = velocity.Length() / MaxPlayerSpeed;
        if (!isOnFloor)
        {
            // Reduce bob effect intensity when the player is mid-air
            speedFactor *= MidAirBobReductionFactor;
        }

        // Update the sway direction
        UpdateSwayDirection(velocity.X, delta);

        // Calculate the bobbing (Y axis) and swaying (X and Z axes)
        float timeFactor = (float)_bobAndSwayTime;
        Vector3 position = new Vector3
        {
            Y = Mathf.Sin(timeFactor * _bobFrequency) * _bobAmplitude * speedFactor,
            X = Mathf.Sin(timeFactor * _swayFrequency) * _swayAmplitude * _swayDirection * speedFactor,
            Z = Mathf.Cos(timeFactor * _swayFrequency) * (_swayAmplitude * 0.25f) * Mathf.Sign(velocity.Z) * speedFactor
        };

        return position;
    }

    // Updates the sway direction smoothly based on the player's movement along the X-axis
    private void UpdateSwayDirection(float velocityX, double delta)
    {
        float targetSwayDirection = velocityX != 0 ? Mathf.Sign(velocityX) : _swayDirection;
        _swayDirection = Mathf.Lerp(_swayDirection, targetSwayDirection, _cameraPositionResetSpeed * (float)delta);
    }

    // Applies the calculated bob and sway offset to the camera container's position
    private void ApplyBobAndSwayOffset(Vector3 newOffset)
    {
        // Remove the previous bob and sway effect
        _cameraComponent.CameraContainer.Position -= _lastBobAndSwayOffset;

        // Apply the new offset
        _cameraComponent.CameraContainer.Position += newOffset;

        // Store the new offset for future updates
        _lastBobAndSwayOffset = newOffset;
    }

    // Handles rotational sway (subtle tilt based on lateral movement)
    private void HandleRotationalSway(double delta)
    {
        float targetRotationZ;
        if (_cameraComponent.GetInputDirection().X > 0)
            targetRotationZ = Mathf.DegToRad(-_swayRotationLimit); // Sway left
        else if (_cameraComponent.GetInputDirection().X < 0)
            targetRotationZ = Mathf.DegToRad(_swayRotationLimit); // Sway right
        else
            targetRotationZ = _originalCameraContainerTransform.Basis.GetEuler().Z; // Return to neutral

        _cameraComponent.CameraContainer.Rotation = new Vector3(
            _cameraComponent.CameraContainer.Rotation.X,
            _cameraComponent.CameraContainer.Rotation.Y,
            Mathf.LerpAngle(_cameraComponent.CameraContainer.Rotation.Z, targetRotationZ, _cameraRotationResetSpeed * (float)delta)
        );
    }
}