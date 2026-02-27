using System;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Core.Scripting.Combat;

public class EquipmentSlot : IReadOnlyEquipmentSlot
{
    public EquipmentCategory Category { get; }


    #region equipment property
    private FirearmEnum? _equipmentEnum;
    public FirearmEnum? EquipmentEnum 
    { 
        get => _equipmentEnum; 
        set 
        {

            if (_equipmentEnum != value) SlotValueChangedEvent?.Invoke(value);
            _equipmentEnum = value; 
        }
    }
    public FirearmType? Equipment
    {
        get => EquipmentTypes.TryGet(EquipmentEnum, out var equipment) ? equipment : null;
    }
    AbstractEquipmentType<FirearmEnum>? IReadOnlyEquipmentSlot.Equipment => Equipment;


    // notification event
    public event Action<FirearmEnum?>? SlotValueChangedEvent; 
    #endregion


    public EquipmentSlot(EquipmentCategory category, FirearmEnum equipment)
    {
        Category = category;
        _equipmentEnum = equipment;
    }

}
