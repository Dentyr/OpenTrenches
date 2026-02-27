
// namespace 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using OpenTrenches.Common.Ability;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Core.Scripting.Combat;

namespace OpenTrenches.Core.Scripting.Player;

public class Character : IIdObject
{
    //* Identification

    public ushort ID { get; }
    public int Team { get; }
    public IClientState ClientState { get; }
    public CharacterState ActionState { get; private set; }


    //* World

    public Vector3 Position { get; set; }


    //* combat

    private float _health = 1;
    public float Health 
    { 
        get => _health;
        private set
        {
            bool wasAlive = IsActive;
            _health = value;
            bool isAlive  = IsActive;

            if (wasAlive != isAlive)
            {
                if (isAlive) ActivatedEvent?.Invoke();
                else InactivatedEvent?.Invoke();
            }
        }
    }

    public bool IsActive => _health > 0;
    public event Action? InactivatedEvent;
    public event Action? ActivatedEvent;
    

    private EquipmentEnum _primary;
    public EquipmentEnum Primary 
    { 
        get => _primary; 
        private set
        {
            if (_primary != value) OnPrimaryChangedEvent?.Invoke(value);
            _primary = value; 
        }
    }
    public Action<EquipmentEnum>? OnPrimaryChangedEvent;

    private ActivatedAbility[] _abilities { get; } = [new ActivatedAbility(AbilityRecords.StimulantAbility)]; //TODO change when new abilities are added
    public IActivatedAbility GetAbility(int index) => _abilities[index];
    public IReadOnlyList<ActivatedAbility> GetAbilities() => _abilities;
    // public event Action<int> AbilityActivated;

    public Character(ushort ID, int Team, IClientState ClientState, Vector3 Position, float Health)
    {
        this.ID = ID;
        this.Team = Team;
        this.ClientState = ClientState;
        this.Position = Position;
        this.Health = Health;
    }

    //* Modifying state

    public void ActivateAbility(int index)
    {
        if (index >= 0 && index < _abilities.Length) _abilities[index].ActivateTimer();
        #if DEBUG
        else throw new IndexOutOfRangeException($"index was {index}, abilities between 0 and {_abilities.Length - 1}");
        #endif
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
            case CharacterAttribute.State:
                ActionState = Serialization.Deserialize<CharacterState>(update.Payload);
                break;
            case CharacterAttribute.PrimarySlot:
                Primary = Serialization.Deserialize<EquipmentEnum>(update.Payload);
                break;
        }
    }

    //* equality

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Character chara) return false;
        // same ID within the same context
        return chara.ID == ID && chara.ClientState == ClientState;
    }
    
    public override int GetHashCode() => HashCode.Combine(ClientState.GetHashCode(), ID);
}