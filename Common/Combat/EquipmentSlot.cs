using System;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Common.Combat;

public class EquipmentSlot<T> : IReadOnlyEquipmentSlot<T>, IReadOnlyEquipmentSlot
    where T : class
{
    public EquipmentCategory Category { get; }


    #region equipment property
        private EquipmentType<T>? _equipment;
        public EquipmentType<T>? Equipment
        {
            get => _equipment;
            set
            {
                var old = _equipment;
                _equipment = value;
                if (old != _equipment) SlotValueChanged?.Invoke(_equipment);
            }
        }
        AbstractEquipmentType? IReadOnlyEquipmentSlot.Equipment => Equipment;

        // notification event
        public event Action<EquipmentType<T>?>? SlotValueChanged; 
        event Action<AbstractEquipmentType?>? IReadOnlyEquipmentSlot.SlotValueChanged
        {
            add => SlotValueChanged += value;
            remove => SlotValueChanged -= value;
        }
    #endregion


    public EquipmentSlot(EquipmentCategory category, EquipmentType<T>? equipment)
    {
        Category = category;
        _equipment = equipment;
    }

}

public interface IReadOnlyEquipmentSlot
{
    public EquipmentCategory Category { get; }
    public AbstractEquipmentType? Equipment { get; }
    public event Action<AbstractEquipmentType?>? SlotValueChanged; 
}
public interface IReadOnlyEquipmentSlot<T> : IReadOnlyEquipmentSlot
    where T : class
{
    public new EquipmentType<T>? Equipment { get; }

    public new event Action<EquipmentType<T>?>? SlotValueChanged; 
}