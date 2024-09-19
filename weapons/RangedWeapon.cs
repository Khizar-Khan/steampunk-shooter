using System;
using Godot;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.weapons;

public partial class RangedWeapon : Weapon
{
    // Internal Attributes
    private int _currentReserveSize;
    private int _currentMagazineSize;

    public override void Initialise(WeaponData weaponData)
    {
        base.Initialise(weaponData);

        if (WeaponData is RangedWeaponData rangedWeaponData)
            InitialiseAmmo(rangedWeaponData);
        else
            throw new InvalidCastException("Invalid WeaponData type assigned to RangedWeapon.");
    }

    private void InitialiseAmmo(RangedWeaponData rangedWeaponData)
    {
        _currentReserveSize = rangedWeaponData.MaxReserveSize;
        _currentMagazineSize = rangedWeaponData.MaxMagazineSize;
    }

    public override void Attack()
    {
        if (_currentMagazineSize > 0)
        {
            _currentMagazineSize--;
            GD.Print($"{Name} Shooting. AMMO RESERVE {_currentMagazineSize} / {_currentReserveSize}");
        }
    }
}