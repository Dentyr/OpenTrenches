
// namespace 
using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scripting.Adapter;
namespace OpenTrenches.Server.Scripting.Player;

public class Character(ushort ID) : IIdObject
{
    public ushort ID { get; } = ID;

    public Vector3 Position { get; set; } = new (0, 10, 0);

    private readonly UpdateableProperty<Vector3> _direction = new(Vector3.Zero);
    /// <summary>
    /// The location this character is looking towards
    /// </summary>
    /// <value></value>
    public Vector3 Direction
    {
        get => _direction.Value;
        set => _direction.Value = value;
    }

    public Vector3 Movement { get; set; } = Vector3.Zero;

    private readonly UpdateableProperty<float> _health = new(10);
    public float Health 
    { 
        get => _health;
        set => _health.Value = value;
    }

    private UpdateableProperty<CharacterState> _state = new(CharacterState.Idle); 
    public CharacterState State
    { 
        get => _state.Value;
        set => _state.Value = value;
    }

    public readonly UpdateableProperty<float> _cooldown = new(0);
    public float Cooldown
    { 
        get => _cooldown;
        set => _cooldown.Value = value;
    }

    public event Action<Character, Vector3>? FireEvent;

    /// <summary>
    /// Called when the adapter simulates time passing
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="adapter"></param>
    public void AdapterSimulate(float delta, ICharacterAdapter adapter)
    {
        Cooldown -= delta;
        if (Cooldown < 0) 
        {
            if (State == CharacterState.Shooting)
            {
                if (Position != Direction)
                {
                    var target = Position + ((Direction - Position).Normalized() * 1000);
                    Character? hit = adapter.AdaptFire(target);
                    if (hit is not null)
                    {
                        hit._health.Value -= 4;
                    }
                    FireEvent?.Invoke(this, target);
                    Cooldown = 0.1f;
                }
            }
        }
    }

    public void TrySwitch(CharacterState newState)
    {
        switch (newState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Aiming:
                break;
            case CharacterState.Shooting:
                if (State == CharacterState.Reloading) return;
                break;
            case CharacterState.Reloading:
                break;
        }
        State = newState;
    }

    //* Updates

    public AbstractUpdateDTO GetUpdate(CharacterAttribute type)
    {
        byte[]? payload = null;
        switch (type)
        {
            case CharacterAttribute.Position:
                payload = Serialization.Serialize(Position);
                break;
            case CharacterAttribute.Health:
                payload = Serialization.Serialize(Health);
                break;
            case CharacterAttribute.Direction:
                payload = Serialization.Serialize(Direction);
                break;
            case CharacterAttribute.Cooldown:
                payload = Serialization.Serialize(Cooldown);
                break;
            case CharacterAttribute.State:
                payload = Serialization.Serialize(State);
                break;
        }
        if (payload is null) throw new Exception();
        return new CharacterUpdateDTO(type, payload, ID);
    }

    public IEnumerable<AbstractUpdateDTO> PollUpdates()
    {
        if (_health.PollChanged()) yield return GetUpdate(CharacterAttribute.Health);
        if (_direction.PollChanged()) yield return GetUpdate(CharacterAttribute.Direction);
        if (_state.PollChanged()) yield return GetUpdate(CharacterAttribute.State);
    }
}