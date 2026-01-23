
// namespace 
using System;
using Godot;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Core.Scripting.Player;

public class Character : IIdObject
{
    public Vector3 Position { get; set; } = new (0, 10, 0);

    private float _health = 10; //TODO debug value
    public float Health 
    { 
        get => _health; 
        set
        {
            if (_health > 0 && value <= 0) DiedEvent?.Invoke();
            else if (_health <= 0 && value > 0) RespawnEvent?.Invoke();
            _health = value;
        }
    }

    public event Action? DiedEvent;
    public event Action? RespawnEvent;

    public ushort ID { get; }

    public Character(ushort ID, Vector3 Position, float Health)
    {
        DiedEvent += () => Console.WriteLine("Died");
        RespawnEvent += () => Console.WriteLine("Respawn");
        this.ID = ID;
        this.Position = Position;
        this.Health = Health;
    }

    //* Updates

    public void Update(CharacterUpdateDTO update)
    {
        switch (update.Attribute)
        {
            case CharacterAttribute.Position:
                Position = Serialization.Deserialize<Vector3>(update.Payload);
                break;
            case CharacterAttribute.Health:
                Health = Serialization.Deserialize<float>(update.Payload);
                break;
        }
    }
}