using Godot;
using SteampunkShooter.components;

namespace SteampunkShooter.player;

public partial class PlayerCharacterBody : CharacterBody3D
{
    private InputComponent _inputComponent;
    private MovementComponent _movementComponent;

    public override void _Ready()
    {
        _inputComponent = GetNode<InputComponent>("InputComponent");
        _movementComponent = GetNode<MovementComponent>("MovementComponent");

        _inputComponent.Connect(
            InputComponent.SignalName.MovementInput,
            new Callable(_movementComponent, nameof(MovementComponent.OnMovementInput))
        );
        
        _inputComponent.Connect(
            InputComponent.SignalName.SprintRequested,
            new Callable(_movementComponent, nameof(MovementComponent.OnSprintRequested))
        );

        _inputComponent.Connect(
            InputComponent.SignalName.JumpRequested,
            Callable.From(_movementComponent.OnJumpRequested)
        );
    }
}