using System;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Common.Combat;

public interface IReadOnlyEquipmentSlot<T> where T : struct, Enum
{
    public EquipmentCategory Category { get; }
    public T? EquipmentEnum { get; }
}
public interface IReadOnlyFirearmSlot : IReadOnlyEquipmentSlot<FirearmEnum>
{
    public FirearmType? Equipment { get; }
    public float ReloadCooldown { get; }
    public float FireCooldown { get; }
    public int AmmoLoaded { get; }
    public int AmmoStored { get; }
}
