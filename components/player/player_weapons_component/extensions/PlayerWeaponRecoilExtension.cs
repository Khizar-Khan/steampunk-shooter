using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.components.weapons_component.extensions;

public partial class PlayerWeaponRecoilExtension : ComponentExtension
{
    [ExportCategory("References")]
    [Export] private Node3D _recoilNode;
    
    [ExportCategory("Settings")]
    [Export] private float _recoilStrength = 0.25f;
    [Export] private float _recoilRotationStrength = 12.5f;
    [Export] private float _positionRecoverySpeed = 1.0f;
    [Export] private float _rotationRecoverySpeed = 60f;

    // Internal Attributes
    private PlayerWeaponsComponent _playerWeaponsComponent;
    private Vector3 _initialPosition;
    private Vector3 _initialRotation;

    private Vector3 _recoilPositionOffset = Vector3.Zero;
    private Vector3 _recoilRotationOffset = Vector3.Zero;
    
    internal override void OnInitialise()
    {
        base.OnInitialise();

        if (_recoilNode == null)
        {
            GD.PrintErr("WeaponRecoilExtension: _recoilNode is null.");
            IsEnabled = false;
            return;
        }

        _initialPosition = _recoilNode.Position;
        _initialRotation = _recoilNode.RotationDegrees;

        _playerWeaponsComponent = ParentComponent as PlayerWeaponsComponent;
        if (_playerWeaponsComponent == null)
        {
            GD.PrintErr("WeaponRecoilExtension: _weaponsComponent is null.");
            IsEnabled = false;
            return;
        }

        SignalBus.Instance.Connect(nameof(SignalBus.Instance.PlayerHasAttacked), new Callable(this, nameof(OnHasAttacked)));
    }
    
    internal override void OnPhysicsProcess(double delta)
    {
        base.OnPhysicsProcess(delta);
        
        // Recover recoil offsets towards zero smoothly over time
        _recoilPositionOffset = _recoilPositionOffset.MoveToward(Vector3.Zero, _positionRecoverySpeed * (float)delta);
        _recoilRotationOffset = _recoilRotationOffset.MoveToward(Vector3.Zero, _rotationRecoverySpeed * (float)delta);

        // Update the recoil node's position and rotation
        _recoilNode.Position = _initialPosition + _recoilPositionOffset;
        _recoilNode.RotationDegrees = _initialRotation + _recoilRotationOffset;
    }
    
    // Signal Event Handlers
    public void OnHasAttacked()
    {
        _recoilPositionOffset.Z += _recoilStrength;
        _recoilRotationOffset.X += _recoilRotationStrength;
    }
}