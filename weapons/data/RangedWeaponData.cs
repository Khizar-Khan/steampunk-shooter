using Godot;

namespace SteampunkShooter.weapons.data;

[GlobalClass]
public partial class RangedWeaponData : WeaponData
{
    public enum FireMode
    {
        SemiAutomatic,
        FullyAutomatic
    }
    
    [ExportCategory("Ranged Specification")]
    [Export] public int MaxReserveSize = 300;
    [Export] public int MaxMagazineSize = 30;
    [Export] public float FireRate = 1.0f;
    [Export] public float ReloadTime = 2.5f; // Default: 2.5 seconds to reload
    [Export] public FireMode FiringMode = FireMode.FullyAutomatic;
}