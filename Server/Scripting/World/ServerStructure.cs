using System;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

public class ServerStructure : IWorldObject
{
    public int Id { get; }
    public Team Team { get; }

    public Vector2I Position { get; }
    Vector2 IWorldObject.Position => Position;

    public StructureEnum Enum { get; }

    public float Hp { get; private set; }
    public bool Destroyed => Hp <= 0;


    /// <summary>
    /// Event when <see cref="Hp"/> reaches 0.
    /// </summary>
    public event Action? DestroyedEvent;


    public ServerStructure(int Id, Team Team, StructureType Type, Vector2I Position)
    {
        this.Id = Id;
        this.Team = Team;
        Enum = Type.Enum;
        
        this.Position = Position;
        Hp = Type.Hp;
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