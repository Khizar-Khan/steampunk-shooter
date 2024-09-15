using Godot;

namespace SteampunkShooter.data;

[GlobalClass]
public partial class WeaponData : Resource
{
    [ExportCategory("References")]
    [Export] public PackedScene ModelScene;

    [ExportCategory("Orientation")]
    [Export] public Vector3 Position;
    [Export] public Vector3 RotationDegrees;

    [ExportCategory("Specification")]
    [Export] public StringName Identification;
    [Export] public int Damage;
    [Export] public float CooldownTime;
    [Export] public float Range;
}