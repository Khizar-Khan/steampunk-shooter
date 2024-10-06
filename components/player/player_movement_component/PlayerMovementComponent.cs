using System;
using Godot;
using SteampunkShooter.player;
using SteampunkShooter.utility;

namespace SteampunkShooter.components;

public partial class PlayerMovementComponent : Component
{
    public enum SpeedType
    {
        Walk,
        Sprint,
        Crouch
    }
    
    [ExportCategory("Movement Settings")]
    [Export] private float _walkSpeed = 3.0f; // Movement speed while walking.
    [Export] private float _sprintSpeed = 4.5f; // Movement speed while sprinting.
    [Export] private float _crouchSpeed = 1.0f; // Movement speed while crouching.
    [Export] private float _velocityDropOffThreshold = 0.025f; // How slow the player has to be to consider them stopped. This is to combat interpolation where slowing down takes too long to reach 0.

    [ExportCategory("Crouch Settings")]
    [Export] private float _crouchHeight = 0.975f; // Player height when crouching.
    [Export] private float _crouchTransitionSpeed = 7.5f; // Speed of the transitions between standing and crouching.
    [Export] private float _crouchTransitionThreshold = 0.005f; // How close to the target value to snap to it immediately.
    
    [ExportCategory("Jump Settings")]
    [Export] private float _jumpHeight = 1.0f; // How high the player can jump.
    [Export] private float _jumpBufferTime = 0.04f; // How early before landing the player can press jump to actually jump.
    [Export] private float _coyoteTime = 0.1f; // How long after falling the player is still able to jump.
    
    [ExportCategory("Gravity Settings")]
    [Export] private float _fallGravityMultiplier = 2.0f; // How much gravity affects the player when falling.
    [Export] private float _terminalVelocity = -20.0f; // The max speed limit the player can fall at.
    
    [ExportCategory("Ground Movement Settings")]
    [Export] private float _accelerationFactor = 12.0f; // Acceleration factor towards target speed (When moving).
    [Export] private float _decelerationFactor = 14.0f; // Deceleration factor towards target speed (When slowing down).

    [ExportCategory("Air Movement Settings")]
    [Export] private float _airAccelerationResponsiveness = 0.20f; // How much acceleration factor is used (E.G. 0.5 for 50% responsiveness)
    [Export] private float _airDecelerationResponsiveness = 0.15f; // How much deceleration factor is used (E.G. 0.75 for 75% responsiveness)

    // Internal Attributes
    private CharacterBody3D _characterBody;
    private Timer _jumpBufferTimer;
    private Timer _coyoteTimer;

    private float _currentSpeed;
    private Vector2 _inputDirection;
    private Vector2 _previousInputDirection;
    private bool _canStand;
    private bool _hasJumped;
    private bool _isSprintRequested;
    private bool _isMovementRequested;
    private bool _isJumpRequested;
    private bool _isCrouchRequested;

    protected override void OnInitialise()
    {
        base.OnInitialise();

        _currentSpeed = _walkSpeed;
        _canStand = true;
        InitialiseCharacterBody();
        InitialiseTimers();
    }

    private void InitialiseCharacterBody()
    {
        _characterBody = Owner as CharacterBody3D;
        if (_characterBody == null)
            throw new NullReferenceException("MovementComponent's Owner is not of type CharacterBody3D or is null.");
    }

    private void InitialiseTimers()
    {
        _jumpBufferTimer = GDUtil.CreateTimer(this, _jumpBufferTime, nameof(OnJumpBufferTimerTimeout));
        _coyoteTimer = GDUtil.CreateTimer(this, _coyoteTime);
    }

    public void SetSpeed(SpeedType speed)
    {
        _currentSpeed = speed switch
        {
            SpeedType.Walk => _walkSpeed,
            SpeedType.Sprint => _sprintSpeed,
            SpeedType.Crouch => _crouchSpeed,
            _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
        };
    }

    public void ApplyGravity(double delta)
    {
        _characterBody.Velocity += _characterBody.GetGravity() * (float)delta * (_characterBody.Velocity.Y >= 0.0f ? 1.0f : _fallGravityMultiplier);
        if (_characterBody.Velocity.Y < _terminalVelocity)
            _characterBody.Velocity = new Vector3(_characterBody.Velocity.X, _terminalVelocity, _characterBody.Velocity.Z);
    }

    private void AdjustMovement(Vector3 targetVelocity, double delta, float responsiveness)
    {
        _characterBody.Velocity = new Vector3(
            MathUtil.ExponentialInterpolate(_characterBody.Velocity.X, targetVelocity.X, responsiveness, (float)delta),
            _characterBody.Velocity.Y,
            MathUtil.ExponentialInterpolate(_characterBody.Velocity.Z, targetVelocity.Z, responsiveness, (float)delta)
        );
    }

    public void ApplyMovement(Vector3 direction, double delta)
    {
        AdjustMovement(direction * _currentSpeed, delta, _accelerationFactor);
    }

    public void RemoveMovement(double delta)
    {
        AdjustMovement(Vector3.Zero, delta, _decelerationFactor);
    }

    public void ApplyAirMovement(Vector3 direction, double delta)
    {
        AdjustMovement(direction * _currentSpeed, delta, _accelerationFactor * _airAccelerationResponsiveness);
    }

    public void RemoveAirMovement(double delta)
    {
        AdjustMovement(Vector3.Zero, delta, _decelerationFactor * _airDecelerationResponsiveness);
    }

    public void Jump()
    {
        _characterBody.Velocity = new Vector3(
            _characterBody.Velocity.X,
            Mathf.Sqrt(_jumpHeight * 2.0f * Mathf.Abs(_characterBody.GetGravity().Y)),
            _characterBody.Velocity.Z
        );

        _hasJumped = true;
        ResetJumpFlags();
    }

    public void Crouch(float delta, bool isStanding = false)
    {
        float targetHeight = isStanding ? GetStandHeight() : _crouchHeight;
        SignalBus.Instance.EmitSignal(nameof(SignalBus.Instance.PlayerAdjustHeightRequested), targetHeight, _crouchTransitionSpeed, _crouchTransitionThreshold, delta);
    }

    public void MoveAndSlide()
    {
        if (_characterBody.Velocity.Length() != 0 && _characterBody.Velocity.Length() < _velocityDropOffThreshold)
            _characterBody.Velocity = Vector3.Zero;

        _characterBody.MoveAndSlide();
    }

    public bool CanJump()
    {
        return (IsOnFloor() && _isJumpRequested) || (!IsOnFloor() && !_coyoteTimer.IsStopped() && !_hasJumped && _isJumpRequested);
    }

    public bool CanWalk()
    {
        return _isMovementRequested && IsOnFloor();
    }

    public bool CanCrouch()
    {
        return _isCrouchRequested && IsOnFloor();
    }

    public bool CanStand()
    {
        return _canStand;
    }

    public bool CanSprint()
    {
        return _isSprintRequested && IsMoveForwardInputActive() && IsOnFloor();
    }

    private bool IsMoveForwardInputActive()
    {
        return _inputDirection.Y < -0.6;
    }

    public bool IsFalling()
    {
        return !IsOnFloor() && _characterBody.Velocity.Y < 0;
    }

    public bool IsOnFloor()
    {
        return _characterBody.IsOnFloor();
    }

    public bool IsIdle()
    {
        return !_isMovementRequested && IsOnFloor();
    }

    public void StartCoyoteTimer()
    {
        _coyoteTimer.Start();
    }

    public void StopCoyoteTimer()
    {
        _coyoteTimer.Stop();
    }

    public Vector3 GetMovementDirectionFromInput()
    {
        return (_characterBody.Transform.Basis * new Vector3(_inputDirection.X, 0, _inputDirection.Y)).Normalized();
    }

    // TODO: Maybe dont add coupling with player specifically?
    private float GetStandHeight()
    {
        if (_characterBody is PlayerEntity playerCharacterBody)
            return playerCharacterBody.GetCollisionShapeStandHeight();

        throw new Exception("There is no stand height for this character body.");
    }

    private void ResetJumpFlags()
    {
        _isJumpRequested = false;
        _jumpBufferTimer.Stop();
    }

    public void ResetLandingFlags()
    {
        _hasJumped = false;
    }

    // Signal Event Handlers
    public void OnMovementInput(Vector2 inputDirection)
    {
        _inputDirection = inputDirection;

        bool wasMoving = _previousInputDirection != Vector2.Zero;
        bool isMoving = _inputDirection != Vector2.Zero;

        if (wasMoving != isMoving)
            _isMovementRequested = isMoving;

        _previousInputDirection = _inputDirection;
    }

    public void OnSprintRequested(bool isRequested)
    {
        _isSprintRequested = isRequested;
    }

    public void OnJumpRequested()
    {
        _isJumpRequested = true;
        _jumpBufferTimer.Start();
    }

    private void OnJumpBufferTimerTimeout()
    {
        _isJumpRequested = false;
    }

    public void OnCrouchRequested(bool isRequested)
    {
        _isCrouchRequested = isRequested;
    }

    public void OnObstructionAbove(bool isObstructionAbove)
    {
        _canStand = !isObstructionAbove;
    }
}