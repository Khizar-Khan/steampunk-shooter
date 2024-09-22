using Godot;

namespace SteampunkShooter.weapons.data;

public abstract partial class WeaponData : Resource
{
    public enum ActivationMode
    {
        Continuous,
        Single
    }
    
    public enum Category
    {
        Melee,
        Ranged
    }
    
    [ExportCategory("References")]
    [Export] public PackedScene WeaponModelScene;

    [ExportCategory("Orientation")]
    [Export] public Vector3 Position;
    [Export] public Vector3 RotationDegrees;

    [ExportCategory("Specification")]
    [Export] public StringName Identification;
    [Export] public ActivationMode WeaponActivationMode;
    [Export] public Category WeaponCategory;
    [Export] public int Damage;
    [Export] public float Range;
}