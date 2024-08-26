using Godot;
using SteampunkShooter.components;

namespace SteampunkShooter.systems.state_machine.states.movement;

public partial class MovementSprintState : State
{
    private MovementComponent _movementComponent;

    public override void Initialise(StateMachine stateMachine)
    {
        base.Initialise(stateMachine);
        _movementComponent = (MovementComponent)StateMachine.Component;
    }
    
    public override void Enter()
    {
        GD.Print("Entered: " + Name);
        ConnectSignals();
    }

    public override void PhysicsUpdate(double delta)
    {
        _movementComponent.ApplyGravity(delta);
        _movementComponent.ApplyMovement(_movementComponent.CalculateMovementDirection());
        _movementComponent.MoveAndSlide();
    }

    public override void Exit()
    {
        GD.Print("Exited: " + Name);
        DisconnectSignals();
    }
    
    // Utility
    private void ConnectSignals()
    {
        _movementComponent.Connect(
            MovementComponent.SignalName.MovementRequested,
            new Callable(this, nameof(OnMovementRequested))
        );
        
        _movementComponent.Connect(
            MovementComponent.SignalName.SprintRequested,
            new Callable(this, nameof(OnSprintRequested))
        );

        _movementComponent.Connect(
            MovementComponent.SignalName.JumpRequested,
            new Callable(this, nameof(OnJumpRequested))
        );
    }

    private void DisconnectSignals()
    {
        _movementComponent.Disconnect(
            MovementComponent.SignalName.MovementRequested,
            new Callable(this, nameof(OnMovementRequested))
        );
        
        _movementComponent.Disconnect(
            MovementComponent.SignalName.SprintRequested,
            new Callable(this, nameof(OnSprintRequested))
        );

        _movementComponent.Disconnect(
            MovementComponent.SignalName.JumpRequested,
            new Callable(this, nameof(OnJumpRequested))
        );
    }
    
    // Signal Event Handlers
    private void OnMovementRequested(bool isRequested)
    {
        if (!isRequested)
            StateMachine.TransitionTo("IdleState");
    }
    
    private void OnSprintRequested(bool isRequested)
    {
        if (_movementComponent.IsOnFloor() && !isRequested)
        {
            StateMachine.TransitionTo("WalkState");
        }
    }
    
    private void OnJumpRequested()
    {
        if (_movementComponent.IsOnFloor())
        {
            StateMachine.TransitionTo("JumpState");
        }
    }
}
