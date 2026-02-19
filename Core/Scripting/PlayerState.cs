using System;

public class PlayerState : IReadOnlyPlayerState
{
    private int _logistics;

    public int Logistics
    {
        get => _logistics;
        set
        {
            _logistics = value;
            OnLogisticsChangedEvent?.Invoke(_logistics);
        }
    }

    // oldValue, newValue
    public event Action<int>? OnLogisticsChangedEvent;
}
public interface IReadOnlyPlayerState
{
    int Logistics { get; }
    event Action<int>? OnLogisticsChangedEvent;
}