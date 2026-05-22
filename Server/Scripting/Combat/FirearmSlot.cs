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
    private const float BaseRecoil = 15f;

    private const float BaseRecoilCooldown = 100f;
    /// <summary>
    /// How much current recoil modifies recoil cooldown speed, as it's easier to broadly correct recoil that percisely return to the original position
    /// </summary>
    private const float RecoilCooldownModifier = 0.4f;

    /// <summary>
    /// Before recoil is applied from firing, the existing recoil will be multiplied by this decay to prevent unbounded recoil growth
    /// </summary>
    private const float RecoilDecay = 0.94f;

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


    public float ReloadCooldown { get; set; }
    public bool Reloaded => ReloadCooldown <= 0;


    public float FireCooldown { get; set; }
    public bool CooledDown => FireCooldown <= 0;

    private readonly UpdateableProperty<int> _ammoLoaded;
    public int AmmoLoaded 
    {
        get => _ammoLoaded.Value;
        set => _ammoLoaded.Value = value;
    }

    private readonly UpdateableProperty<int> _ammoStored;
    public int AmmoStored 
    {
        get => _ammoStored.Value;
        set => _ammoStored.Value = value;
    }

    /// <summary>
    /// Recoil as MoA range 
    /// </summary>
    private readonly UpdateableProperty<float> _recoil;
    public float Recoil
    {
        get => _recoil.Value;
        set => _recoil.Value = value;
    }

    public event Action<FirearmSlotAttribute, byte[]>? AttributeChangedEvent;
    private void PropagateAttributeUpdate<T>(FirearmSlotAttribute attribute, T value)
    {
        AttributeChangedEvent?.Invoke(attribute, Serialization.Serialize(value));
    }

    public FirearmSlot(FirearmType equipment) : base(EquipmentCategory.Firearm, equipment.Id)
    {
        _ammoLoaded = new(x => PropagateAttributeUpdate(FirearmSlotAttribute.AmmoLoaded, x));
        _ammoStored = new(x => PropagateAttributeUpdate(FirearmSlotAttribute.AmmoStored, x));
        _recoil = new(x => PropagateAttributeUpdate(FirearmSlotAttribute.Recoil, x));

        SetAmmoToDefault(equipment);
    }

    /// <summary>
    /// Reduces fire cooldown, reload cooldown, and recoil. Not <paramref name="aiming"/> will reduce maximum recoil recovery and speed of recovery
    /// </summary>
    /// <param name="delta"></param>
    public void Cooldown(float delta, bool aiming)
    {
        float targetRecoil = aiming ? 0 : BaseRecoil;
        float recoverModifier = aiming ? 1f : 0.3f;

        if (ReloadCooldown > 0) ReloadCooldown -= delta;
        else if (FireCooldown > 0) FireCooldown -= delta;
        if (Recoil > targetRecoil) 
        {
            Recoil -= (BaseRecoilCooldown + RecoilCooldownModifier * Recoil) * delta * recoverModifier;
            if (Recoil < 0) Recoil = 0;
        }
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


    [MemberNotNullWhen(true, nameof(Equipment))]
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
            Recoil *= RecoilDecay;
            Recoil += Equipment.Stats.Recoil;
        }

    }
}
