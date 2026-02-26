using System;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Core.Scripting.Combat;

public class EquipmentSlot<T> : IReadOnlyEquipmentSlot<T>, IReadOnlyEquipmentSlot
    where T : class
{
    public EquipmentCategory Category { get; }


    #region equipment property
    private EquipmentEnum? _equipmentEnum;
    public EquipmentEnum? EquipmentEnum 
    { 
        get => _equipmentEnum; 
        set 
        {

            if (_equipmentEnum != value) SlotValueChangedEvent?.Invoke(value);
            _equipmentEnum = value; 
        }
    }
    public EquipmentType<T>? Equipment
    {
        get => EquipmentTypes.TryGet<T>(EquipmentEnum, out var equipment) ? equipment : null;
    }
    AbstractEquipmentType? IReadOnlyEquipmentSlot.Equipment => Equipment;


    // notification event
    public event Action<EquipmentEnum?>? SlotValueChangedEvent; 
    #endregion


    public EquipmentSlot(EquipmentCategory category, EquipmentEnum equipment)
    {
        Category = category;
        _equipmentEnum = equipment;
    }

}