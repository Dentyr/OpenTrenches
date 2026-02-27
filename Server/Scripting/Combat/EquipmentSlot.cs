using System;
using OpenTrenches.Common.Combat;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Server.Scripting.Combat;

public abstract class EquipmentSlot<T> : IReadOnlyEquipmentSlot<T> where T : struct, Enum
{
    public EquipmentCategory Category { get; }


    #region equipment property
    private UpdateableProperty<T?> _equipment = new();
    public bool PollEquipmentUpdate() => _equipment.PollChanged();
    public T? EquipmentEnum
    {
        get => _equipment.Value;
        protected set => _equipment.Value = value;
    }


    // notification event
    public event Action<FirearmType?>? SlotValueChangedEvent; 
    #endregion


    protected EquipmentSlot(EquipmentCategory category, T equipment)
    {
        Category = category;
        EquipmentEnum = equipment;
    }

}
