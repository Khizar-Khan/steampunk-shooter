using Godot;
using Godot.Collections;

namespace SteampunkShooter.weapons.data;

[GlobalClass]
public partial class WeaponDatabase : Resource
{
    [Export] private Array<WeaponData> _weaponDataArray = new();

    // Internal Attributes
    private Dictionary<StringName, WeaponData> _weaponDataDictionary;

    // This method initialises the dictionary only once and ensures immutability afterward
    private void InitialiseDictionary()
    {
        if (_weaponDataDictionary != null)
            return;

        _weaponDataDictionary = new Dictionary<StringName, WeaponData>();
        foreach (WeaponData weaponData in _weaponDataArray)
        {
            if (!_weaponDataDictionary.ContainsKey(weaponData.Identification))
                _weaponDataDictionary.Add(weaponData.Identification, weaponData);
            else
                GD.PrintErr($"Duplicate weapon ID found: {weaponData.Identification}. Ignoring duplicate.");
        }

        _weaponDataArray.Clear();
        _weaponDataArray = null;
    }

    // Public method to access WeaponData by ID
    public WeaponData GetWeaponData(string weaponIdentification)
    {
        InitialiseDictionary();

        if (_weaponDataDictionary.TryGetValue(weaponIdentification, out WeaponData weaponData))
            return weaponData;

        GD.PrintErr($"Weapon ID '{weaponIdentification}' not found in WeaponDatabase.");
        return null;
    }

    // Public method to get the weapon scene directly
    public PackedScene GetWeaponScene(string weaponIdentification)
    {
        WeaponData weaponData = GetWeaponData(weaponIdentification);
        return weaponData?.WeaponModelScene;
    }
}