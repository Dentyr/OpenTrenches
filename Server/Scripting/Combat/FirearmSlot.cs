using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Server.Scripting.Combat;

public class FirearmSlot : EquipmentSlot<FirearmEnum>, IReadOnlyFirearmSlot
{
    public FirearmType? Equipment
    {
        get => EquipmentTypes.TryGet(EquipmentEnum, out var equipment) ? equipment : null;
        set 
        {
            
            if (EquipmentEnum != value?.Id)
            {
                EquipmentEnum = value?.Id;
                if (Equipment is FirearmType firearmType) SetAmmoToDefault(firearmType);
            }
        }
    }
    private void SetAmmoToDefault(FirearmType firearmType)
    {
        AmmoLoaded = firearmType.Stats.MagazineSize;
        AmmoStored = AmmoLoaded * 5;
    }


    private readonly UpdateableProperty<float> _reload = new();
    public float ReloadCooldown 
    {
        get => _reload.Value;
        set => _reload.Value = value;
    }

    public bool Reloaded => ReloadCooldown <= 0;

    private readonly UpdateableProperty<float> _cooldown = new();
    public float FireCooldown 
    {
        get => _cooldown.Value;
        set => _cooldown.Value = value;
    }

    public bool CooledDown => FireCooldown <= 0;

    private readonly UpdateableProperty<int> _ammoLoaded = new();
    public int AmmoLoaded 
    {
        get => _ammoLoaded.Value;
        set => _ammoLoaded.Value = value;
    }

    private readonly UpdateableProperty<int> _ammoStored = new();
    public int AmmoStored 
    {
        get => _ammoStored.Value;
        set => _ammoStored.Value = value;
    }

    /// <summary>
    /// Recoil as MoA range 
    /// </summary>
    private readonly UpdateableProperty<float> _recoil = new();
    public float Recoil
    {
        get => _recoil.Value;
        set => _recoil.Value = value;
    }


    public void Cooldown(float delta)
    {
        if (ReloadCooldown > 0) ReloadCooldown -= delta;
        else if (FireCooldown > 0) FireCooldown -= delta;
    }

    /// <summary>
    /// Sets timers to reload, if able
    /// </summary>
    /// <returns>True if set to reloading state</returns>
    public bool TryReload()
    {
        if (Equipment is not null && Reloaded)
        {
            // Load until out of stored ammo and mag is full.
            var ammoToLoad = Math.Min(
                AmmoStored,
                Equipment.Stats.MagazineSize - AmmoLoaded
            );
            if (ammoToLoad > 0)
            {
                ReloadCooldown = Equipment.Stats.ReloadSeconds;
                FireCooldown = 0;

                AmmoLoaded += ammoToLoad;
                AmmoStored -= ammoToLoad;
                return true;
            }

        }
        return false;
    }


    public bool CanFire()
    {
        return EquipmentEnum is not null && Reloaded && CooledDown && AmmoLoaded > 0;
    }
    /// <summary>
    /// Sets fire cooldown, if able
    /// </summary>
    /// <returns>True if set to fire cooldown</returns>
    public void UseBullet()
    {
        if (AmmoLoaded > 0) AmmoLoaded -= 1;
    }
    public void ApplyCooldownAndRecoil()
    {
        if (Equipment is not null)
        {
            FireCooldown = 60 / Equipment.Stats.RateOfFire;
            Recoil *= 0.9f;
            Recoil += Equipment.Stats.Recoil;
        }

    }


    public FirearmSlot(FirearmType equipment) : base(EquipmentCategory.Firearm, equipment.Id)
    {
        SetAmmoToDefault(equipment);
    }


    //* Updates

    public byte[] GetUpdate(FirearmSlotAttribute type)
    {
        byte[]? payload = null;
        switch (type)
        {
            case FirearmSlotAttribute.AmmoLoaded:
                payload = Serialization.Serialize(AmmoLoaded);
                break;
            case FirearmSlotAttribute.AmmoStored:
                payload = Serialization.Serialize(AmmoStored);
                break;
            case FirearmSlotAttribute.Recoil:
                payload = Serialization.Serialize(Recoil);
                break;
        }
        if (payload is null) throw new Exception();
        return payload;
    }

    public IEnumerable<KeyValuePair<FirearmSlotAttribute, byte[]>> PollUpdates()
    {
        if (_ammoLoaded.PollChanged()) yield return new KeyValuePair<FirearmSlotAttribute, byte[]>(FirearmSlotAttribute.AmmoLoaded, GetUpdate(FirearmSlotAttribute.AmmoLoaded));
        if (_ammoStored.PollChanged()) yield return new KeyValuePair<FirearmSlotAttribute, byte[]>(FirearmSlotAttribute.AmmoStored, GetUpdate(FirearmSlotAttribute.AmmoStored));
    }
}
