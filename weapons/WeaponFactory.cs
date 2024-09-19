using Godot;
using SteampunkShooter.data;

namespace SteampunkShooter.weapons;

public partial class WeaponFactory : RefCounted
{
    public static Weapon CreateWeapon(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            GD.PrintErr("WeaponData is null. Cannot create weapon.");
            return null;
        }
        
        if (weaponData.ModelScene == null)
        {
            GD.PrintErr($"Weapon '{weaponData.Identification}' has no model scene assigned.");
            return null;
        }

        Node instance = weaponData.ModelScene.Instantiate();
        if (instance is not Weapon weapon)
        {
            GD.PrintErr($"Failed to instantiate weapon from model scene. The scene is not of type 'Weapon'.");
            return null;
        }
        
        weapon.Initialise(weaponData);
        weapon.Position = weaponData.Position;
        weapon.RotationDegrees = weaponData.RotationDegrees;

        // TODO: Set weapon-specific properties if any (e.g., Damage, CooldownTime, Range)

        return weapon;
    }
}