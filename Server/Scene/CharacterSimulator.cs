using System;
using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scene.World;

public partial class CharacterSimulator : CharacterBody3D, ICharacterAdapter
{
    public Character Character { get; }
    private IWorldSimulator World { get; } 
    public CharacterSimulator(Character Character, IWorldSimulator World)
    {
        //* World DI
        this.World = World;

        //* character
        this.Character = Character;
        Position = Character.Position;

        //* collision
        AddChild(new CollisionShape3D()
        {
            Shape = new BoxShape3D()
            {
                Size = new(1, 2, 1),
            }
        });
        CollisionLayer = SceneDefines.Map.CharacterLayer;
        CollisionMask = SceneDefines.Map.AllMask;
    }

    public override void _Process(double delta)
    {        
        Position = Character.Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        //* movement
        Move((float)delta);

        //* ability

        Character.AdapterSimulate((float)delta, this);
    }
    private void Move(float delta)
    {
        
        Position = Character.Position;
        Velocity += SceneDefines.Physics.g * delta;
        Velocity = new(Character.Movement.X * delta, Velocity.Y, Character.Movement.Z * delta);
        MoveAndSlide();
        Character.Position = Position;
    }

    Character? ICharacterAdapter.AdaptFire(Vector3 target)
    {
        var hits = GetViewport().World3D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters3D()
        {
            From = Character.Position,
            To = target,
            CollisionMask = SceneDefines.Map.CharacterLayer,
        });
        if (hits.Count == 0) return null;
        else if (hits[SceneDefines.PhysicsKey.Collider].AsGodotObject() is CharacterSimulator hitsim)
        {
            return hitsim.Character;
        }
        return null;
    }

    void ICharacterAdapter.AdaptBuild(Vector2I cell, TileType buildTarget, float progress)
    {
        World.Build(cell, buildTarget, progress);
    }
}
