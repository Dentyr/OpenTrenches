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
            if(!EqualityComparer<T>.Default.Equals(_value, value)) _changed = true;
            _value = value;
        }
    }
    private bool _changed;

    public UpdateableProperty() : this(default!) {}
    public UpdateableProperty(T Value)
    {
        _value = Value;
        _changed = false;
    }

    public bool PollChanged() 
    {
        if (_changed)
        {
            _changed = false;
            return true;
        }
        return false;
    }
    
    public static implicit operator T(UpdateableProperty<T> prop) => prop.Value;
}