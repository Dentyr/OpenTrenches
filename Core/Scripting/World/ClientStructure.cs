using System;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.Teams;

namespace OpenTrenches.Core.Scripting.World;

public class ClientStructure
{   
    public int Id { get; }
    public int Team { get; }

    public Vector2I Position { get; }

    public StructureEnum Enum { get; }

    public float Health { get; private set; }

    public event Action? DestroyedEvent;


    public ClientStructure(int Id, int Team, StructureType Type, Vector2I Position, float Health)
    {
        this.Id = Id;
        this.Team = Team;
        Enum = Type.Enum;
        
        this.Position = Position;
        this.Health = Health;
    }

}