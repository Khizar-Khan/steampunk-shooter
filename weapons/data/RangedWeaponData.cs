using Godot;

namespace SteampunkShooter.weapons.data;

[GlobalClass]
public partial class RangedWeaponData : WeaponData
{
    [ExportCategory("Ranged Specification")]
    [Export] public int MaxReserveSize = 300;
    [Export] public int MaxMagazineSize = 30;
    [Export] public float FireRate = 1.0f;
    [Export] public float ReloadTime = 2.5f;
}