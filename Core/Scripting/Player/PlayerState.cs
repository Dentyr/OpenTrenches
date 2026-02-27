using System;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Core.Scripting.Combat;

namespace OpenTrenches.Core.Scripting.Player;

public class PlayerState : IReadOnlyPlayerState
{
    public FirearmState PrimarySlotState { get; } = new();
    IReadOnlyFirearmState? IReadOnlyPlayerState.PrimarySlotState => PrimarySlotState;

    private int _logistics;

    public int Logistics
    {
        get => _logistics;
        set
        {
            if (_logistics != value) OnLogisticsChangedEvent?.Invoke(_logistics);
            _logistics = value;
        }
    }


    public event Action<int>? OnLogisticsChangedEvent;

    
    public void Update(FirearmSlotUpdateDTO update)
    {
        PrimarySlotState.Update(update);
    }

    public void Update(PlayerUpdateDTO update)
    {
        switch (update.Attribute)
        {
            case PlayerAttribute.Logistics:
                Logistics = Serialization.Deserialize<int>(update.Payload);
                break;
        }
    }
}
public interface IReadOnlyPlayerState
{
    int Logistics { get; }
    IReadOnlyFirearmState? PrimarySlotState { get; }

    event Action<int>? OnLogisticsChangedEvent;

}