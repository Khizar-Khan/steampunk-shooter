using Godot;
using SteampunkShooter.components;

namespace SteampunkShooter.systems.state_machine.states.movement;

public partial class MovementJumpState : State
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
        _movementComponent.Jump();
    }

    public override void Update(double delta)
    {
        if (!_movementComponent.IsOnFloor())
            return;

        StateMachine.TransitionTo(_movementComponent.CalculateMovementDirection() != Vector3.Zero ? "WalkState" : "IdleState");
    }

    public override void PhysicsUpdate(double delta)
    {
        _movementComponent.ApplyGravity(delta);

        Vector3 direction = _movementComponent.CalculateMovementDirection();
        if (direction != Vector3.Zero)
            _movementComponent.ApplyMovement(direction);
        else
            _movementComponent.RemoveMovement();
        _movementComponent.MoveAndSlide();
    }

    public override void Exit()
    {
        GD.Print("Exited: " + Name);
    }
}