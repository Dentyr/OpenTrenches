using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;


namespace OpenTrenches.Common.Combat;

public static class EquipmentTypes
{
    private static readonly Dictionary<EquipmentEnum, AbstractEquipmentType> _all = new()
    {
        {
            EquipmentEnum.Rifle,
            new EquipmentType<FirearmStats>(
                EquipmentEnum.Rifle,
                EquipmentCategory.Firearm,
                logisticsCost: 5,
                new FirearmStats
                {
                    DamagePerProjectile = 30,
                    RateOfFire = 30f,
                    ReloadSeconds = 2f,
                    MagazineSize = 15,
                    ProjectilesPerShot = 1
                })
        },
        {
            EquipmentEnum.Shotgun,
            new EquipmentType<FirearmStats>(
                EquipmentEnum.Shotgun,
                EquipmentCategory.Firearm,
                logisticsCost: 25,
                new FirearmStats
                {
                    DamagePerProjectile = 70,
                    RateOfFire = 30f,
                    ReloadSeconds = 8f,
                    MagazineSize = 4,
                    ProjectilesPerShot = 8,
                })
        },
        {
            EquipmentEnum.MachineGun,
            new EquipmentType<FirearmStats>(
                EquipmentEnum.MachineGun,
                EquipmentCategory.Firearm,
                logisticsCost: 50,
                new FirearmStats
                {
                    DamagePerProjectile = 22,
                    RateOfFire = 120f,
                    ReloadSeconds = 2f,
                    MagazineSize = 60,
                    ProjectilesPerShot = 1
                })
        }
    };

    public static IReadOnlyDictionary<EquipmentEnum, AbstractEquipmentType> All { get; } = new ReadOnlyDictionary<EquipmentEnum, AbstractEquipmentType>(_all);

    public static AbstractEquipmentType Get(EquipmentEnum type) =>
        _all.TryGetValue(type, out var equipment) ? equipment : throw new ArgumentException($"unregistered equipment: {type}");

    private static EquipmentType<TStats> Get<TStats>(EquipmentEnum type) where TStats : class
    {
        var equipment = Get(type);
        if (equipment is EquipmentType<TStats> equipmentTyped) return equipmentTyped;
        throw new Exception($"Attempted to make convert {equipment.GetType()} into {typeof(EquipmentType<TStats>)}");
    }

    public static EquipmentType<FirearmStats> Rifle => Get<FirearmStats>(EquipmentEnum.Rifle);
    public static EquipmentType<FirearmStats> Shotgun => Get<FirearmStats>(EquipmentEnum.Shotgun);
    public static EquipmentType<FirearmStats> MachineGun => Get<FirearmStats>(EquipmentEnum.MachineGun);

    public static IEnumerable<AbstractEquipmentType> GetAllInCategory(EquipmentCategory category) =>
        _all.Values.Where(e => e.Category == category);
}