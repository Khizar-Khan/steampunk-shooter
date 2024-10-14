using System;
using Godot;

namespace SteampunkShooter.scripts.cameras;

public partial class CameraControl : Node3D
{
    [Export]
    private CollisionShape3D _collisionShapeInstrument;

    private CapsuleShape3D _capsuleShapeInstrument;
    private Node3D _parentNode;
    private float _eyeLevelPercentage;

    public override void _Ready()
    {
        if (_collisionShapeInstrument == null)
            throw new NullReferenceException("CollisionShape3D is null.");

        _capsuleShapeInstrument = _collisionShapeInstrument.Shape as CapsuleShape3D;
        if (_capsuleShapeInstrument == null)
            throw new NullReferenceException("Shape is not a CapsuleShape3D.");

        _parentNode = GetParent<Node3D>();
        if (_parentNode == null)
            throw new NullReferenceException("Parent node is null.");

        _eyeLevelPercentage = Position.Y / _capsuleShapeInstrument.Height; // Dynamically set eye level percentage
    }

    public override void _Process(double delta)
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (_capsuleShapeInstrument == null)
            return;

        Vector3 newPosition = Position;
        newPosition.Y = _capsuleShapeInstrument.Height * _eyeLevelPercentage;
        Position = newPosition;
    }
}