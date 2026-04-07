using System;
using OpenTrenches.Common.Combat;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Server.Scripting.Combat;

public abstract class EquipmentSlot<T> : IReadOnlyEquipmentSlot<T> where T : struct, Enum
{
    public EquipmentCategory Category { get; }


    #region equipment property
    private UpdateableProperty<T?> _equipment;
    public event Action<T?>? EquipmentUpdateEvent;
    public T? EquipmentEnum
    {
        get => _equipment.Value;
        protected set => _equipment.Value = value;
    }

    #endregion


    protected EquipmentSlot(EquipmentCategory category, T equipment)
    {
        _equipment = new(x => EquipmentUpdateEvent?.Invoke(x));

        Category = category;
        EquipmentEnum = equipment;
    }

}
