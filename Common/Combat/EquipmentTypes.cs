using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
                    ProjectileDistance = 40f,

                    DamagePerProjectile = 10f,

                    SpreadMOA = 3,
                    RateOfFire = 140f,
                    ReloadSeconds = 2.5f,
                    MagazineSize = 15,
                    Recoil = 70,
                    ProjectilesPerShot = 1,
                }
            )
            {
                Name = nameof(FirearmEnum.Rifle),
                Description = "Single Shot Rifle",
            }
        },
        {
            FirearmEnum.Shotgun,
            new FirearmType(
                FirearmEnum.Shotgun,
                logisticsCost: 25,
                new FirearmStats
                {   //based on WWI issue trench gun
                    ProjectileDistance = 17f,

                    DamagePerProjectile = 8f,
                    SpreadMOA = 400,
                    RateOfFire = 80f,
                    ReloadSeconds = 3.75f,
                    MagazineSize = 6,
                    Recoil = 200,
                    ProjectilesPerShot = 8,
                }
            )
            {
                Name = nameof(FirearmEnum.Shotgun),
                Description = "Multi Shot Firearm",
            }
        },
        {
            FirearmEnum.MachineGun,
            new FirearmType(
                FirearmEnum.MachineGun,
                logisticsCost: 50,
                new FirearmStats
                {
                    ProjectileDistance = 55f,

                    DamagePerProjectile = 15,

                    SpreadMOA = 5,
                    RateOfFire = 500f,
                    ReloadSeconds = 2f,
                    MagazineSize = 60,
                    Recoil = 45,
                    ProjectilesPerShot = 1,
                }
            )
            {
                Name = nameof(FirearmEnum.MachineGun),
                Description = "Rapid fire firearm",
            }
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
    public static bool TryGet(FirearmEnum? type, [NotNullWhen(true)] out FirearmType? equipment)
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