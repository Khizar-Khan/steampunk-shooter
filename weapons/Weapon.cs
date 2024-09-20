using System;
using Godot;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.weapons;

public abstract partial class Weapon : Node3D
{
    public WeaponData WeaponData { get; private set; }

    public virtual void Initialise(WeaponData weaponData)
    {
        WeaponData = weaponData;
        if (WeaponData == null)
            throw new NullReferenceException("WeaponData is null.");

        ConfigureWeapon(weaponData);
    }
    
    protected virtual void ConfigureWeapon(WeaponData weaponData)
    {
        Position = weaponData.Position;
        RotationDegrees = weaponData.RotationDegrees;
    }

    public abstract void Attack();
}