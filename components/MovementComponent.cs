using System;
using Godot;
using SteampunkShooter.systems.state_machine;

namespace SteampunkShooter.components;

public partial class MovementComponent : Component
{
    [Signal]
    public delegate void MovementRequestedEventHandler(bool isRequested);
    
    [Signal]
    public delegate void SprintRequestedEventHandler(bool isRequested);
    
    [Signal]
    public delegate void JumpRequestedEventHandler();
    
    [Export] private float _jumpVelocity = 4.5f;
    [Export] private float _speed = 4.0f;

    private CharacterBody3D _characterBody;
    private StateMachine _stateMachine;
    private Vector2 _inputDirection;
    private Vector2 _previousInputDirection;

    protected override void Initialise()
    {
        _characterBody = Owner as CharacterBody3D;
        if (_characterBody == null)
            throw new NullReferenceException("MovementComponent's Owner is not of type CharacterBody3D or is null.");
        
        _stateMachine = GetNode<StateMachine>("StateMachine");
        if (_stateMachine == null)
            throw new NullReferenceException("MovementComponent's state machine is null.");
    }

    public void ApplyGravity(double delta)
    {
        _characterBody.Velocity += _characterBody.GetGravity() * (float)delta;
    }

    public void ApplyMovement(Vector3 direction)
    {
        _characterBody.Velocity = new Vector3(direction.X * _speed, _characterBody.Velocity.Y, direction.Z * _speed);
    }

    public void RemoveMovement()
    {
        _characterBody.Velocity = new Vector3(Mathf.MoveToward(_characterBody.Velocity.X, 0, _speed), _characterBody.Velocity.Y, Mathf.MoveToward(_characterBody.Velocity.Z, 0, _speed));
    }
    
    public void Jump()
    {
        _characterBody.Velocity = new Vector3(_characterBody.Velocity.X, _jumpVelocity, _characterBody.Velocity.Z);
    }

    public bool IsOnFloor()
    {
        return _characterBody.IsOnFloor();
    }

    public void MoveAndSlide()
    {
        _characterBody.MoveAndSlide();
    }
    
    // Utility
    public Vector3 CalculateMovementDirection()
    {
        return (_characterBody.Transform.Basis * new Vector3(_inputDirection.X, 0, _inputDirection.Y)).Normalized();
    }

    // Signal Event Handlers
    public void OnMovementInput(Vector2 inputDirection)
    {
        _inputDirection = inputDirection;
        
        bool wasMoving = _previousInputDirection != Vector2.Zero;
        bool isMoving = _inputDirection != Vector2.Zero;
        
        if (wasMoving != isMoving)
            EmitSignal(SignalName.MovementRequested, isMoving);
        
        _previousInputDirection = _inputDirection;
    }
    
    public void OnSprintRequested(bool isRequested)
    {
        EmitSignal(SignalName.SprintRequested, isRequested);
    }
    
    public void OnJumpRequested()
    {
        EmitSignal(SignalName.JumpRequested);
    }
}