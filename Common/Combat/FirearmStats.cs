namespace OpenTrenches.Common.Combat;

public record class FirearmStats
{
    public EquipmentCategory Category => EquipmentCategory.Firearm;

    public int DamagePerProjectile { get; init; }
    public int ProjectilesPerShot { get; init; } = 1;
    public float SpreadAngleDegrees { get; init; }
    public float RateOfFire { get; init; }
    public float ReloadSeconds { get; init; }
    public int MagazineSize { get; init; }
}
