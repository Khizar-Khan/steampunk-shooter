using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.components.weapons_component.extensions;

public partial class WeaponRecoilExtension : ComponentExtension
{
    [ExportCategory("References")]
    [Export] private Node3D _recoilNode;
    
    [ExportCategory("Settings")]
    [Export] private float _recoilStrength = 5f;
    [Export] private float _recoverySpeed = 10f;
    [Export] private float _recoilRotationAmount = 5f;

    // Internal Attributes
    private WeaponsComponent _weaponsComponent;
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

        _weaponsComponent = ParentComponent as WeaponsComponent;
        if (_weaponsComponent == null)
        {
            GD.PrintErr("WeaponRecoilExtension: _weaponsComponent is null.");
            IsEnabled = false;
        }
    }
    
    
}