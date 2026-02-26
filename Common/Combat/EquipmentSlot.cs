using System;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Common.Combat;

public interface IReadOnlyEquipmentSlot
{
    public EquipmentCategory Category { get; }
    public AbstractEquipmentType? Equipment { get; }
    public EquipmentEnum? EquipmentEnum { get; }
}
public interface IReadOnlyEquipmentSlot<T> : IReadOnlyEquipmentSlot
    where T : class
{
    public new EquipmentType<T>? Equipment { get; }
}

public interface IReadOnlyFirearmSlot : IReadOnlyEquipmentSlot<FirearmStats>
{
    public float ReloadCooldown { get; }
    public float FireCooldown { get; }
    public int AmmoLoaded { get; }
    public int AmmoStored { get; }
}