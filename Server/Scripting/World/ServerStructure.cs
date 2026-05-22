using System;
using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Adapter;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

public class ServerStructure : IWorldObject
{
    public int Id { get; }
    public Team Team { get; }

    public Vector2I Position { get; }
    Vector2 IWorldObject.Position => Position;

    public StructureEnum Enum { get; }

    private readonly UpdateableProperty<float> _hp;
    public float Hp
    {
        get => _hp;
        private set => _hp.Value = value;
    }
    public bool Destroyed => _hp <= 0;


    /// <summary>
    /// Event when <see cref="_hp"/> reaches 0.
    /// </summary>
    public event Action? DestroyedEvent;

    public event Action<StructureUpdateDTO>? StructureUpdateEvent;
    private void PropagateUpdate<T>(StructureAttribute type, T value)
        => StructureUpdateEvent?.Invoke(new StructureUpdateDTO(type, Serialization.Serialize(value), Id));

    public ServerStructure(int Id, Team Team, StructureType Type, Vector2I Position)
    {
        this.Id = Id;
        this.Team = Team;
        Enum = Type.Enum;
        
        this.Position = Position;
        _hp = new(Type.Hp, x => PropagateUpdate(StructureAttribute.Health, x));
    }

    public void TakeDamage(float damage) => Hp -= damage;

    /// <summary>
    /// The area this structure spans in the world space
    /// </summary>
    public Rect2I GetProfile() => StructureTypes.Get(Enum).Profile.Translate(Position);

    public void ApplyDamage(float damage)
    {
        if (Hp > 0)
        {
            Hp -= damage;
            if (Hp <= 0) DestroyedEvent?.Invoke();
        }
    }
}