using System;
using OpenTrenches.Common.Combat;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Server.Scripting.Combat;

public class EquipmentSlot : IReadOnlyEquipmentSlot
{
    public EquipmentCategory Category { get; }


    #region equipment property
    private UpdateableProperty<FirearmEnum?> _equipment = new();
    public bool PollEquipmentUpdate() => _equipment.PollChanged();
    public FirearmEnum? EquipmentEnum
    {
        get => _equipment.Value;
    }
    public FirearmType? Equipment
    {
        get => EquipmentTypes.TryGet(EquipmentEnum, out var equipment) ? equipment : null;
        set => _equipment.Value = value?.Id;
    }
    AbstractEquipmentType? IReadOnlyEquipmentSlot.Equipment => Equipment;


    // notification event
    public event Action<FirearmType?>? SlotValueChangedEvent; 
    #endregion


    public EquipmentSlot(EquipmentCategory category, FirearmType? equipment)
    {
        Category = category;
        Equipment = equipment;
    }

}