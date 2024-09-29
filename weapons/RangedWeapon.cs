using System;
using Godot;
using Godot.Collections;
using SteampunkShooter.utility;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.weapons;

public partial class RangedWeapon : Weapon
{
    // Internal Attributes
    private RangedWeaponData _rangedWeaponData;
    private int _currentReserveSize;
    private int _currentMagazineSize;
    private Timer _fireRateTimer;
    public Timer ReloadTimer;

    public override void Initialise(WeaponData weaponData)
    {
        base.Initialise(weaponData);
        _rangedWeaponData = weaponData as RangedWeaponData ?? throw new InvalidCastException("Invalid WeaponData type.");
        InitialiseAmmo(_rangedWeaponData);
        InitialiseTimers(_rangedWeaponData);
    }

    private void InitialiseAmmo(RangedWeaponData rangedWeaponData)
    {
        _currentReserveSize = rangedWeaponData.MaxReserveSize;
        _currentMagazineSize = rangedWeaponData.MaxMagazineSize;
    }
    
    private void InitialiseTimers(RangedWeaponData rangedWeaponData)
    {
        _fireRateTimer = GDUtil.CreateTimer(this, 1 / rangedWeaponData.FireRate);
        ReloadTimer = GDUtil.CreateTimer(this, rangedWeaponData.ReloadTime, nameof(OnReloadTimerTimeout));
    }

    public override void Attack()
    {
        if (_currentMagazineSize > 0 && _fireRateTimer.IsStopped() && ReloadTimer.IsStopped())
        {
            _currentMagazineSize--;
            _fireRateTimer.Start();
            GD.Print($"{Name} Ammo: {_currentMagazineSize} / {_currentReserveSize}");
            
            Dictionary hitScanResult = GDUtil.PerformHitScanFromScreenCenter(this, _rangedWeaponData.Range);
            
            if(hitScanResult != null && hitScanResult.Count > 0)
                GD.Print(hitScanResult["collider"]);
        }
    }
    
    public bool CanReload()
    {
        return ReloadTimer.IsStopped() && _rangedWeaponData.MaxMagazineSize != _currentMagazineSize;
    }
    
    public void Reload()
    {
        ReloadTimer.Start();
        GD.Print($"{Name} Reloading.");
    }
    
    // Signal Event Handlers
    private void OnReloadTimerTimeout()
    {
        int ammoNeeded = _rangedWeaponData.MaxMagazineSize - _currentMagazineSize;
        int ammoToReload = Math.Min(ammoNeeded, _currentReserveSize);

        _currentMagazineSize += ammoToReload;
        _currentReserveSize -= ammoToReload;

        GD.Print($"{Name} Reloaded. Ammo: {_currentMagazineSize} / {_currentReserveSize}");
    }
}