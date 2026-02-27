using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace OpenTrenches.Common.Combat;

public static class EquipmentTypes
{
    private static readonly Dictionary<FirearmEnum, FirearmType> _all = new()
    {
        {
            FirearmEnum.Rifle,
            new FirearmType(
                FirearmEnum.Rifle,
                logisticsCost: 5,
                new FirearmStats
                {
                    DamagePerProjectile = 10f,
                    SpreadMOA = 3,
                    RateOfFire = 70f,
                    ReloadSeconds = 3f,
                    MagazineSize = 15,
                    Recoil = 75,
                    ProjectilesPerShot = 1,
                })
        },
        {
            FirearmEnum.Shotgun,
            new FirearmType(
                FirearmEnum.Shotgun,
                logisticsCost: 25,
                new FirearmStats
                {   //based on WWI issue trench gun
                    DamagePerProjectile = 2.5f,
                    SpreadMOA = 120,
                    RateOfFire = 45f,
                    ReloadSeconds = 5f,
                    MagazineSize = 6,
                    Recoil = 200,
                    ProjectilesPerShot = 8,
                })
        },
        {
            FirearmEnum.MachineGun,
            new FirearmType(
                FirearmEnum.MachineGun,
                logisticsCost: 50,
                new FirearmStats
                {
                    DamagePerProjectile = 22,
                    SpreadMOA = 5,
                    RateOfFire = 500f,
                    ReloadSeconds = 2f,
                    MagazineSize = 60,
                    Recoil = 30,
                    ProjectilesPerShot = 1,
                })
        }
    };

    public static IReadOnlyDictionary<FirearmEnum, FirearmType> All { get; } = new ReadOnlyDictionary<FirearmEnum, FirearmType>(_all);


    public static bool TryGet<T>(T? type, out FirearmType? equipment) where T : struct, Enum
    {
        equipment = null;
        if (type is null) return false;
        if (type is FirearmEnum firearmEnum) return TryGet(firearmEnum, out equipment);
        return false;
    }
    public static bool TryGet(FirearmEnum? type, out FirearmType? equipment)
    {
        if (type is not FirearmEnum notnull)
        {
            equipment = null;
            return false;
        }
        equipment = Get(notnull);
        return true;
    }
    public static FirearmType Get(FirearmEnum type) =>
        _all.TryGetValue(type, out var equipment) ? equipment : throw new ArgumentException($"unregistered equipment: {type}");

    public static FirearmType Rifle => Get(FirearmEnum.Rifle);
    public static FirearmType Shotgun => Get(FirearmEnum.Shotgun);
    public static FirearmType MachineGun => Get(FirearmEnum.MachineGun);

    public static IEnumerable<FirearmType> GetAllInCategory(EquipmentCategory category) =>
        _all.Values.Where(e => e.Category == category);
}