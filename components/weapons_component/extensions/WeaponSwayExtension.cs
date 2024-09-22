using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.components.weapons_component.extensions;

public partial class WeaponSwayExtension : ComponentExtension
{
    [ExportCategory("References")]
    [Export] private Node3D _swayNode;
    
    [ExportCategory("Settings")]
    [Export] private Vector2 _minimum = new(-20.0f, -20.0f);
    [Export] private Vector2 _maximum = new(20.0f, 20.0f);
    [Export] private float _positionSpeed = 0.07f;
    [Export] private float _positionAmount = 0.05f;
    [Export] private float _rotationSpeed = 0.1f;
    [Export] private float _rotationAmount = 15.0f;

    // Internal Attributes
    private WeaponsComponent _weaponsComponent;
    private Vector3 _initialPosition;
    private Vector3 _initialRotation;

    public override void Initialise()
    {
        base.Initialise();

        if (_swayNode == null)
        {
            GD.PrintErr("WeaponSwayExtension: _swayNode is null.");
            IsEnabled = false;
            return;
        }

        _initialPosition = _swayNode.Position;
        _initialRotation = _swayNode.RotationDegrees;

        _weaponsComponent = ParentComponent as WeaponsComponent;
        if (_weaponsComponent == null)
        {
            GD.PrintErr("WeaponSwayExtension: _weaponsComponent is null.");
            IsEnabled = false;
        }
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        Sway((float)delta);
    }

    private void Sway(float delta)
    {
        // Clamp mouse movement
        _weaponsComponent.MouseDelta = new Vector2(
            Mathf.Clamp(_weaponsComponent.MouseDelta.X, _minimum.X, _maximum.X),
            Mathf.Clamp(_weaponsComponent.MouseDelta.Y, _minimum.Y, _maximum.Y)
        );

        // Calculate new weapon position based on mouse movement
        Vector3 newPosition = _swayNode.Position;
        newPosition.X = Mathf.Lerp(_swayNode.Position.X, _initialPosition.X - (_weaponsComponent.MouseDelta.X * _positionAmount) * delta, _positionSpeed);
        newPosition.Y = Mathf.Lerp(_swayNode.Position.Y, _initialPosition.Y + (_weaponsComponent.MouseDelta.Y * _positionAmount) * delta, _positionSpeed);

        // Update the weapon position
        _swayNode.Position = newPosition;

        // Calculate new weapon rotation based on mouse movement
        Vector3 newRotationDegrees = _swayNode.RotationDegrees;
        newRotationDegrees.Y = Mathf.Lerp(_swayNode.RotationDegrees.Y, _initialRotation.Y + (_weaponsComponent.MouseDelta.X * -_rotationAmount) * delta, _rotationSpeed);
        newRotationDegrees.X = Mathf.Lerp(_swayNode.RotationDegrees.X, _initialRotation.X - (_weaponsComponent.MouseDelta.Y * _rotationAmount) * delta, _rotationSpeed);
        
        // Update the weapon rotation
        _swayNode.RotationDegrees = newRotationDegrees;
        
        // Reset Mouse Delta
        _weaponsComponent.MouseDelta = Vector2.Zero;
    }
}