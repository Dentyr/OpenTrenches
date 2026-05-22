namespace OpenTrenches.Common.Combat;

public record class FirearmStats
{
    public EquipmentCategory Category => EquipmentCategory.Firearm;

    public float DamagePerProjectile { get; init; }
    public float ProjectileDistance { get; init; } = 1000f;
    public float RateOfFire { get; init; }
    public float ReloadSeconds { get; init; }

    public int MagazineSize { get; init; }

    public int ProjectilesPerShot { get; init; } = 1;
    public float SpreadMOA { get; init; }
    public int Recoil { get; init; }


    public float FirePerSecond => RateOfFire > 0 ? 60 / RateOfFire : 0;
}
