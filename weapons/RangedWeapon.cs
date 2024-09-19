using System;
using Godot;
using SteampunkShooter.data;

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
        PerformFireAction();
    }

    private void PerformFireAction()
    {
        if (!CanFire())
            return;

        FireWeapon();
    }

    private bool CanFire()
    {
        return HasAmmoInMagazine();
    }

    private bool HasAmmoInMagazine()
    {
        return _currentMagazineSize > 0;
    }

    private void FireWeapon()
    {
        GD.Print("RangedWeapon fired");

        // TODO: Implement projectile spawning or hitscan logic here

        _currentMagazineSize--;
        // TODO: Trigger animation and sound effects
    }

    public void PerformReloadAction()
    {
        // TODO: Implement reload logic
    }
}