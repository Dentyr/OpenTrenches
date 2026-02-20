using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Server.Scripting.Combat;

public class FirearmSlot : EquipmentSlot<FirearmStats>
{
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

    private readonly UpdateableProperty<float> _ammoLoaded = new();
    public float AmmoLoaded 
    {
        get => _ammoLoaded.Value;
        set => _ammoLoaded.Value = value;
    }

    private readonly UpdateableProperty<float> _ammoStored = new();
    public float AmmoStored 
    {
        get => _ammoStored.Value;
        set => _ammoStored.Value = value;
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
                FireCooldown = 60 / Equipment.Stats.RateOfFire;

                AmmoLoaded += ammoToLoad;
                AmmoStored -= ammoToLoad;
                return true;
            }

        }
        return false;
    }

    /// <summary>
    /// Sets fire cooldown, if able
    /// </summary>
    /// <returns>True if set to fire cooldown</returns>
    public bool TryShoot()
    {
        if (Equipment is not null && Reloaded && CooledDown)
        {
            FireCooldown = 60 / Equipment.Stats.RateOfFire;
            AmmoLoaded -= 1;
            return true;
        }
        return false;
    }


    public FirearmSlot(EquipmentCategory category, EquipmentType<FirearmStats> equipment) : base(category, equipment)
    {
    }


    //* Updates

    public byte[] GetUpdate(FirearmSlotAttribute type)
    {
        byte[]? payload = null;
        switch (type)
        {
            case FirearmSlotAttribute.Reload:
                payload = Serialization.Serialize(ReloadCooldown);
                break;
            case FirearmSlotAttribute.Cooldown:
                payload = Serialization.Serialize(FireCooldown);
                break;
            case FirearmSlotAttribute.AmmoLoaded:
                payload = Serialization.Serialize(AmmoLoaded);
                break;
            case FirearmSlotAttribute.AmmoStored:
                payload = Serialization.Serialize(AmmoStored);
                break;
        }
        if (payload is null) throw new Exception();
        return payload;
    }

    public IEnumerable<KeyValuePair<FirearmSlotAttribute, byte[]>> PollUpdates()
    {
        if (_reload.PollChanged()) yield return new KeyValuePair<FirearmSlotAttribute, byte[]>(FirearmSlotAttribute.Reload, GetUpdate(FirearmSlotAttribute.Reload));
        if (_cooldown.PollChanged()) yield return new KeyValuePair<FirearmSlotAttribute, byte[]>(FirearmSlotAttribute.Cooldown, GetUpdate(FirearmSlotAttribute.Cooldown));
        if (_ammoLoaded.PollChanged()) yield return new KeyValuePair<FirearmSlotAttribute, byte[]>(FirearmSlotAttribute.AmmoLoaded, GetUpdate(FirearmSlotAttribute.AmmoLoaded));
        if (_ammoStored.PollChanged()) yield return new KeyValuePair<FirearmSlotAttribute, byte[]>(FirearmSlotAttribute.AmmoStored, GetUpdate(FirearmSlotAttribute.AmmoStored));
    }
}