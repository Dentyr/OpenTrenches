
// namespace 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using OpenTrenches.Common.Ability;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Core.Scripting.Player;

public class Character : IIdObject
{
    //* Identification

    public ushort ID { get; }
    public CharacterState State { get; private set; }


    //* World

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

    public float MaxHealth => 15;

    public event Action? DiedEvent;
    public event Action? RespawnEvent;
    


    public ActivatedAbility[] _abilities { get; } = [new ActivatedAbility(AbilityRecords.StimulantAbility)]; //TODO change when new abilities are added
    public IActivatedAbility GetAbility(int index) => _abilities[index];
    public IReadOnlyList<IActivatedAbility> GetAbilities() => [.. _abilities.Cast<IActivatedAbility>()];
    // public event Action<int> AbilityActivated;

    public Character(ushort ID, Vector3 Position, float Health)
    {
        DiedEvent += () => Console.WriteLine("Died");
        RespawnEvent += () => Console.WriteLine("Respawn");
        this.ID = ID;
        this.Position = Position;
        this.Health = Health;
    }

    //* Modifying state

    public void ActivateAbility(int index)
    {
        if (index >= 0 && index < _abilities.Length) 
        {
            _abilities[index].ActivateTimer();
        }
    }

    //* Process
    
    /// <summary>
    /// For client-side processing, such as ticking down timers
    /// </summary>
    /// <param name="delta"></param>
    public void Process(float delta)
    {
        foreach (var ability in _abilities) ability.ProgressTimer(delta);
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
            case CharacterAttribute.Direction:
                break;
            case CharacterAttribute.Cooldown:
                break;
            case CharacterAttribute.State:
                State = Serialization.Deserialize<CharacterState>(update.Payload);
                break;
        }
    }
}