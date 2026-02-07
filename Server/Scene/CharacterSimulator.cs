using System;
using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scene.World;

public partial class CharacterSimulator : CharacterBody3D, ICharacterAdapter
{
    public Character Character { get; }
    
    private IWorldSimulator World { get; } 
    IWorldSimulator ICharacterAdapter.World => World;

    public CharacterSimulator(Character Character, IWorldSimulator World)
    {
        //* World DI
        this.World = World;

        //* character
        this.Character = Character;
        Position = Character.Position;
        
        Character.DiedEvent += Deactivate;
        Character.RespawnEvent += Activate;

        //* collision
        AddChild(new CollisionShape3D()
        {
            Shape = new BoxShape3D()
            {
                Size = new(1, 2, 1),
            }
        });
        Activate();
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
        Velocity = new(Character.MovementVelocity.X, Velocity.Y, Character.MovementVelocity.Z);
        Velocity += SceneDefines.Physics.g * delta;
        MoveAndSlide();
        Character.Position = Position;
    }

    FireHitResult ICharacterAdapter.AdaptFire(Vector3 target)
    {
        var hits = GetViewport().World3D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters3D()
        {
            From = Character.Position,
            To = target,
            CollisionMask = SceneDefines.Map.BulletMask,
        });
        // hit nothing
        if (hits.Count == 0) return new FireHitResult.Miss(target);
        // hit character
        else if (hits[SceneDefines.PhysicsKey.Collider].AsGodotObject() is CharacterSimulator hitsim)
            return new FireHitResult.Hit(hits[SceneDefines.PhysicsKey.Position].AsVector3(), hitsim.Character);
        // hit something else
        return new FireHitResult.Miss(hits[SceneDefines.PhysicsKey.Position].AsVector3());
    }

    /// <summary>
    /// Stop all physics interactions
    /// </summary>
    private void Deactivate()
    {
        CollisionLayer = 0;
        CollisionMask = 0;
        SetPhysicsProcess(false);
    }

    /// <summary>
    /// Enables physics simulation
    /// </summary>
    private void Activate()
    {
        CollisionLayer = SceneDefines.Map.CharacterLayer;
        CollisionMask = SceneDefines.Map.TerrainLayer;
        SetPhysicsProcess(true);
    }
}
