using System;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

public class ServerStructure
{
    public int Id { get; }
    public Team Team { get; }

    public Vector2I Position { get; }

    public StructureEnum Enum { get; }

    public float Health { get; private set; }

    public event Action? DestroyedEvent;


    public ServerStructure(int Id, Team Team, StructureType Type, Vector2I Position)
    {
        this.Id = Id;
        this.Team = Team;
        Enum = Type.Enum;
        
        this.Position = Position;
        Health = Type.HitPoints;
    }

    public void TakeDamage(float damage) => Health -= damage;

    /// <summary>
    /// The area this structure spans in the world space
    /// </summary>
    public Rect2I GetProfile() => StructureTypes.Get(Enum).Profile.Translate(Position);

    public void ApplyDamage(float damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            GD.Print(Health);
            if (Health <= 0) DestroyedEvent?.Invoke();
        }
    }
}