using System;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Common.Combat;

public interface IReadOnlyEquipmentSlot
{
    public EquipmentCategory Category { get; }
    public AbstractEquipmentType<FirearmEnum>? Equipment { get; }
    public FirearmEnum? EquipmentEnum { get; }
}
public interface IReadOnlyFirearmSlot : IReadOnlyEquipmentSlot
{
    public new FirearmType? Equipment { get; }
    public float ReloadCooldown { get; }
    public float FireCooldown { get; }
    public int AmmoLoaded { get; }
    public int AmmoStored { get; }
}
