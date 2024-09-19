using System;
using Godot;
using SteampunkShooter.data;

namespace SteampunkShooter.weapons;

public abstract partial class Weapon : Node3D
{
    public WeaponData WeaponData { get; private set; }
    
    public virtual void Initialise(WeaponData weaponData)
    {
        WeaponData = weaponData;
        if (WeaponData == null)
            throw new NullReferenceException("WeaponData is null.");
        
        // TODO: Maybe don't hide from here?
        Hide();
    }

    public abstract void Attack();
}