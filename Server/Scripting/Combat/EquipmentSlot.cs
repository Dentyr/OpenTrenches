using System;
using System.Collections;
using OpenTrenches.Common.Combat;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Server.Scripting.Combat;

public class EquipmentSlot<T> : IReadOnlyEquipmentSlot<T>, IReadOnlyEquipmentSlot
    where T : class
{
    public EquipmentCategory Category { get; }


    #region equipment property
    private UpdateableProperty<EquipmentEnum?> _equipment = new();
    public bool PollEquipmentUpdate() => _equipment.PollChanged();
    public EquipmentEnum? EquipmentEnum
    {
        get => _equipment.Value;
    }
    public EquipmentType<T>? Equipment
    {
        get => EquipmentTypes.TryGet<T>(EquipmentEnum, out var equipment) ? equipment : null;
        set => _equipment.Value = value?.Id;
    }
    AbstractEquipmentType? IReadOnlyEquipmentSlot.Equipment => Equipment;


    // notification event
    public event Action<EquipmentType<T>?>? SlotValueChangedEvent; 
    #endregion


    public EquipmentSlot(EquipmentCategory Category, EquipmentType<T>? Equipment)
    {
        this.Category = Category;
        this.Equipment = Equipment;
    }

}