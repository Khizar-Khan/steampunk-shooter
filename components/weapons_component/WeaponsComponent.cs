using System;
using Godot;
using Godot.Collections;
using SteampunkShooter.utility;
using SteampunkShooter.weapons;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.components;

public partial class WeaponsComponent : Component
{
    [ExportCategory("References")]
    [Export] private WeaponDatabase _weaponDatabase;
    [Export] private Node3D _weaponAttachmentPoint;

    [ExportCategory("Settings")]
    [Export] private int _maxWeaponCount = 3;
    [Export] private float _weaponSwitchCooldownTime = 0.25f;

    // Internal Attributes
    private Array<Weapon> _equippedWeapons;
    public Weapon CurrentWeapon;
    private Timer _switchWeaponTimer;
    private bool _isAttackRequested;
    
    public Vector2 MouseDelta;

    protected override void Initialise()
    {
        base.Initialise();

        if (_weaponDatabase == null || _weaponAttachmentPoint == null)
            throw new NullReferenceException("WeaponComponent is not fully initialized. Please ensure all references are set.");

        _equippedWeapons = new Array<Weapon>();
        _switchWeaponTimer = GDUtil.CreateTimer(this, _weaponSwitchCooldownTime);

        // Testing
        AddWeapon("test 1");
        AddWeapon("test 2");
        AddWeapon("test 3");
        
        if (CurrentWeapon == null && _equippedWeapons.Count > 0)
            SwitchCurrentWeapon(0);
    }

    protected override void Process(double delta)
    {
        base.Process(delta);

        if (_isAttackRequested)
        {
            CurrentWeapon?.Attack();

            if (CurrentWeapon.WeaponData is RangedWeaponData rangedWeaponData && rangedWeaponData.FiringMode == RangedWeaponData.FireMode.SemiAutomatic)
                _isAttackRequested = false;
        }
            
    }

    private bool AddWeapon(StringName weaponIdentifier)
    {
        if (_equippedWeapons.Count >= _maxWeaponCount)
        {
            GD.PrintErr("Maximum number of weapons reached.");
            return false;
        }

        WeaponData weaponData = _weaponDatabase.GetWeaponData(weaponIdentifier);
        if (weaponData == null)
        {
            GD.PrintErr($"Weapon '{weaponIdentifier}' not found in the WeaponDatabase.");
            return false;
        }

        if (IsWeaponAlreadyEquipped(weaponData.Identification))
        {
            GD.PrintErr($"Weapon with ID '{weaponData.Identification}' already equipped.");
            return false;
        }

        AttachWeapon(WeaponFactory.CreateWeapon(weaponData));
        return true;
    }

    private bool RemoveWeapon(StringName weaponIdentifier)
    {
        for (int i = 0; i < _equippedWeapons.Count; i++)
        {
            if (_equippedWeapons[i].WeaponData.Identification == weaponIdentifier)
            {
                Weapon weaponToRemove = _equippedWeapons[i];
                _equippedWeapons.Remove(weaponToRemove);
                weaponToRemove.QueueFree();
                return true;
            }
        }

        GD.PrintErr($"Weapon '{weaponIdentifier}' not found.");
        return false;
    }

    private bool IsWeaponAlreadyEquipped(StringName weaponIdentifier)
    {
        foreach (Weapon weapon in _equippedWeapons)
        {
            if (weapon.WeaponData.Identification == weaponIdentifier)
                return true;
        }

        return false;
    }

    private void AttachWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            GD.PrintErr("Failed to attach weapon. Weapon instance is null.");
            return;
        }
        
        weapon.Hide();
        _weaponAttachmentPoint.AddChild(weapon);
        _equippedWeapons.Add(weapon);
    }

    private void SwitchCurrentWeapon(int index)
    {
        if (!_switchWeaponTimer.IsStopped())
            return;
        
        if (index < 0 || index >= _equippedWeapons.Count)
        {
            GD.PrintErr($"Invalid weapon index {index}. Cannot switch.");
            return;
        }
        
        CurrentWeapon?.Hide();
        CurrentWeapon = _equippedWeapons[index];
        CurrentWeapon.Show();
        
        _switchWeaponTimer.Start();
    }
    
    // Signal Event Handlers
    public void OnNextWeaponRequest()
    {
        if (_equippedWeapons.Count == 0)
        {
            GD.PrintErr("No weapons equipped.");
            return;
        }
        
        int currentIndex = CurrentWeapon == null ? -1 : _equippedWeapons.IndexOf(CurrentWeapon);
        int nextIndex = (currentIndex + 1) % _equippedWeapons.Count;
        
        SwitchCurrentWeapon(nextIndex);
    }
    
    public void OnPreviousWeaponRequest()
    {
        if (_equippedWeapons.Count == 0)
        {
            GD.PrintErr("No weapons equipped.");
            return;
        }
    
        int currentIndex = CurrentWeapon == null ? 0 : _equippedWeapons.IndexOf(CurrentWeapon);
        int previousIndex = (currentIndex - 1 + _equippedWeapons.Count) % _equippedWeapons.Count;
    
        SwitchCurrentWeapon(previousIndex);
    }

    public void OnWeaponAttackRequest(bool isRequested)
    {
        _isAttackRequested = isRequested;
    }
    
    public void OnWeaponReloadRequest()
    {
        if (CurrentWeapon is not RangedWeapon rangedWeapon)
            return;
        
        rangedWeapon.Reload();
    }
    
    // Signal Event Handlers
    public void OnMouseMotion(Vector2 mouseDelta)
    {
        if (!IsEnabled)
            return;

        MouseDelta = mouseDelta;
    }
}