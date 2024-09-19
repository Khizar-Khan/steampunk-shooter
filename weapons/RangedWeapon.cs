using System;
using Godot;
using SteampunkShooter.utility;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.weapons;

public partial class RangedWeapon : Weapon
{
    // Internal Attributes
    private int _currentReserveSize;
    private int _currentMagazineSize;
    
    private Timer _fireRateTimer;
    private Timer _reloadTimer;

    public override void Initialise(WeaponData weaponData)
    {
        base.Initialise(weaponData);

        if (WeaponData is RangedWeaponData rangedWeaponData)
        {
            InitialiseAmmo(rangedWeaponData);
            InitialiseTimers(rangedWeaponData);
        }
        else
            throw new InvalidCastException("Invalid WeaponData type assigned to RangedWeapon.");
    }

    private void InitialiseAmmo(RangedWeaponData rangedWeaponData)
    {
        _currentReserveSize = rangedWeaponData.MaxReserveSize;
        _currentMagazineSize = rangedWeaponData.MaxMagazineSize;
    }
    
    private void InitialiseTimers(RangedWeaponData rangedWeaponData)
    {
        _fireRateTimer = GDUtil.CreateTimer(this, 1 / rangedWeaponData.FireRate);
        _reloadTimer = GDUtil.CreateTimer(this, rangedWeaponData.ReloadTime, nameof(OnReloadTimerTimeout));
    }

    public override void Attack()
    {
        if (_currentMagazineSize > 0 && _fireRateTimer.IsStopped() && _reloadTimer.IsStopped())
        {
            _currentMagazineSize--;
            _fireRateTimer.Start();
            GD.Print($"{Name} Ammo: {_currentMagazineSize} / {_currentReserveSize}");
        }
    }
    
    public void Reload()
    {
        if (_reloadTimer.IsStopped() && (WeaponData as RangedWeaponData).MaxMagazineSize != _currentMagazineSize)
        {
            _reloadTimer.Start();
            GD.Print($"{Name} Reloading.");
        }
    }
    
    // Signal Event Handlers
    private void OnReloadTimerTimeout()
    {
        int ammoNeeded = (WeaponData as RangedWeaponData).MaxMagazineSize - _currentMagazineSize;
        int ammoToReload = Math.Min(ammoNeeded, _currentReserveSize);

        _currentMagazineSize += ammoToReload;
        _currentReserveSize -= ammoToReload;

        GD.Print($"{Name} Reloaded. Ammo: {_currentMagazineSize} / {_currentReserveSize}");
    }
}