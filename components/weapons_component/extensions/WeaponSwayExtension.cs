using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.components.weapons_component.extensions;

public partial class WeaponSwayExtension : ComponentExtension
{
    [Export] private Vector2 _minimum = new Vector2(-20.0f, -20.0f);
    [Export] private Vector2 _maximum = new Vector2(20.0f, 20.0f);
    [Export] private float _positionSpeed = 0.07f;
    [Export] private float _positionAmount = 0.1f;
    [Export] private float _rotationSpeed = 0.1f;
    [Export] private float _rotationAmount = 30.0f;

    WeaponsComponent _weaponsComponent;
    
    public override void Initialise()
    {
        base.Initialise();
        _weaponsComponent = ParentComponent as WeaponsComponent;
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
        Vector3 newPosition = _weaponsComponent.CurrentWeapon.Position;
        newPosition.X = Mathf.Lerp(_weaponsComponent.CurrentWeapon.Position.X, _weaponsComponent.CurrentWeapon.WeaponData.Position.X - (_weaponsComponent.MouseDelta.X * _positionAmount) * delta, _positionSpeed);
        newPosition.Y = Mathf.Lerp(_weaponsComponent.CurrentWeapon.Position.Y, _weaponsComponent.CurrentWeapon.WeaponData.Position.Y + (_weaponsComponent.MouseDelta.Y * _positionAmount) * delta, _positionSpeed);

        // Update the weapon position
        _weaponsComponent.CurrentWeapon.Position = newPosition;

        // Calculate new weapon rotation based on mouse movement
        Vector3 newRotationDegrees = _weaponsComponent.CurrentWeapon.RotationDegrees;
        newRotationDegrees.Y = Mathf.Lerp(_weaponsComponent.CurrentWeapon.RotationDegrees.Y, _weaponsComponent.CurrentWeapon.WeaponData.RotationDegrees.Y + (_weaponsComponent.MouseDelta.X * -_rotationAmount) * delta, _rotationSpeed);
        newRotationDegrees.X = Mathf.Lerp(_weaponsComponent.CurrentWeapon.RotationDegrees.X, _weaponsComponent.CurrentWeapon.WeaponData.RotationDegrees.X - (_weaponsComponent.MouseDelta.Y * _rotationAmount) * delta, _rotationSpeed);

        // Update the weapon rotation
        _weaponsComponent.CurrentWeapon.RotationDegrees = newRotationDegrees;
        
        _weaponsComponent.MouseDelta = Vector2.Zero;
    }
}