using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Diagnostics.CodeAnalysis;


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
                    DamagePerProjectile = 10f,
                    SpreadMOA = 3,
                    RateOfFire = 70f,
                    ReloadSeconds = 3f,
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
                {   //based on WWI issue trench gun
                    DamagePerProjectile = 2.5f,
                    SpreadMOA = 120,
                    RateOfFire = 45f,
                    ReloadSeconds = 5f,
                    MagazineSize = 6,
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
                    SpreadMOA = 5,
                    RateOfFire = 500f,
                    ReloadSeconds = 2f,
                    MagazineSize = 60,
                    ProjectilesPerShot = 1,
                })
        }
    };

    public static IReadOnlyDictionary<EquipmentEnum, AbstractEquipmentType> All { get; } = new ReadOnlyDictionary<EquipmentEnum, AbstractEquipmentType>(_all);


    public static bool TryGet<TStats>(EquipmentEnum? type, [NotNullWhen(true)] out EquipmentType<TStats>? equipment) where TStats : class
    {
        if (type is not EquipmentEnum notnull || Get(notnull) is not EquipmentType<TStats> equipmentType) {
            equipment = null;
            return false;
        }
        equipment = equipmentType;
        return true;
    }
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