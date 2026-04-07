using System;
using System.Collections.Generic;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Server.Scripting.Adapter;

public class UpdateableProperty<T>
{
    private T _value;
    public T Value 
    { 
        get => _value;
        set
        {
            if(!EqualityComparer<T>.Default.Equals(_value, value)) UpdatedEvent?.Invoke(value);
            _value = value;
        }
    }
    private event Action<T> UpdatedEvent;

    public UpdateableProperty(Action<T> UpdateEvent) : this(default!, UpdateEvent) {}

    public UpdateableProperty(T Value, Action<T> UpdateEvent)
    {
        _value = Value;
        UpdatedEvent = UpdateEvent;
    }
    
    public static implicit operator T(UpdateableProperty<T> prop) => prop.Value;
}