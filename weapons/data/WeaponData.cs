using Godot;

namespace SteampunkShooter.weapons.data;

public abstract partial class WeaponData : Resource
{
    public enum Type
    {
        Melee,
        Ranged
    }
    
    [ExportCategory("References")]
    [Export] public PackedScene ModelScene;

    [ExportCategory("Orientation")]
    [Export] public Vector3 Position;
    [Export] public Vector3 RotationDegrees;

    [ExportCategory("Specification")]
    [Export] public StringName Identification;
    [Export] public Type WeaponType;
    [Export] public int Damage;
    [Export] public float Range;
    [Export] public float CooldownTime;
}