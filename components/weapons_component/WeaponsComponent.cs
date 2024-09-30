using System;
using Godot;
using Godot.Collections;
using SteampunkShooter.utility;
using SteampunkShooter.weapons;
using SteampunkShooter.weapons.data;

namespace SteampunkShooter.components.weapons_component;

public partial class WeaponsComponent : Component
{
    [ExportCategory("References")]
    [Export] private WeaponDatabase _weaponDatabase;
    [Export] private Node3D _weaponAttachmentPoint;

    [ExportCategory("Settings")]
    [Export] private int _maxWeaponCount = 3;
    [Export] private Array<StringName> _startingWeapons = new();
    [Export] private float _weaponSwitchCooldownTime = 0.25f;

    // Cached Values
    private Array<Weapon> _equippedWeapons;
    private Timer _switchWeaponTimer;
    
    public Weapon CurrentWeapon { get; private set; }
    public bool IsAttackRequested;
    public bool IsReloadRequested;
    public Vector2 MouseDelta;

    public bool SwitchToNextWeapon;
    public bool SwitchToPreviousWeapon;

    protected override void OnInitialise()
    {
        base.OnInitialise();

        if (_weaponDatabase == null || _weaponAttachmentPoint == null)
            throw new NullReferenceException("WeaponComponent is not fully initialized. Please ensure all references are set.");

        _equippedWeapons = new Array<Weapon>();
        _switchWeaponTimer = GDUtil.CreateTimer(this, _weaponSwitchCooldownTime);

        if (!(_startingWeapons.Count > 0))
        {
            GD.PrintErr("Starting weapons not set.");
            IsEnabled = false;
            return;
        }
        
        StartingWeapons(_startingWeapons);
    }

    private void StartingWeapons(Array<StringName> weaponIdentifiers)
    {
        foreach (StringName weaponIdentifier in weaponIdentifiers)
            AddWeapon(weaponIdentifier);
        
        SwitchCurrentWeapon(0);
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
        
        CurrentWeapon = _equippedWeapons[index];
        _switchWeaponTimer.Start();
    }

    public void EquipNextWeapon()
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
    
    public void EquipPreviousWeapon()
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
    
    // Signal Event Handlers
    public void OnNextWeaponRequest()
    {
        if (!_switchWeaponTimer.IsStopped())
            return;
        
        SwitchToNextWeapon = true;
    }
    
    public void OnPreviousWeaponRequest()
    {
        if (!_switchWeaponTimer.IsStopped())
            return;
        
        SwitchToPreviousWeapon = true;
    }

    public void OnWeaponAttackRequest(bool isRequested)
    {
        IsAttackRequested = isRequested;
    }
    
    public void OnWeaponReloadRequest()
    {
        if (CurrentWeapon is not RangedWeapon)
            return;

        IsReloadRequested = true;
    }
    
    public void OnMouseMotion(Vector2 mouseDelta)
    {
        if (!IsEnabled)
            return;

        MouseDelta = mouseDelta;
    }
}