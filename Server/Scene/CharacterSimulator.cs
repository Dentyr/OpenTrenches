using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Resources;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scene.World;

public partial class CharacterSimulator : CharacterBody2D, ICharacterAdapter
{
    public Character Character { get; }
    /// <summary>
    /// Sets local position and velocity to match <see cref="Character"/>'s
    /// </summary>
    private void SyncPosition()
    {
        Position = Character.Position * CommonDefines.CellSize;
        Velocity = Character.MovementVelocity * CommonDefines.CellSize;
    }
    /// <summary>
    /// Sets <see cref="Character"/>'s position to match local position
    /// </summary>
    private void WritebackPosition()
    {
        Character.Position = Position / CommonDefines.CellSize;
    }

    private IWorldSimulator World { get; } 
    IWorldSimulator ICharacterAdapter.World => World;

    public CharacterSimulator(Character Character, IWorldSimulator World)
    {
        //* World DI
        this.World = World;

        //* character
        this.Character = Character;
        // convert local position to cell location
        SyncPosition();
        
        Character.DiedEvent += Deactivate;
        Character.RespawnEvent += Activate;

        //* collision
        AddChild(new CollisionShape2D()
        {
            Shape = new CircleShape2D()
            {
                Radius = 16,
            }
        });
        Activate();
    }

    public override void _PhysicsProcess(double delta)
    {
        SyncPosition();
        //* movement
        Move((float)delta);

        //* ability

        Character.AdapterSimulate((float)delta, this);
    }
    private void Move(float delta)
    {
        MoveAndSlide();
        WritebackPosition();
    }

    FireHitResult ICharacterAdapter.AdaptFire(Vector2 target)
    {
        var hits = GetViewport().World2D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
            From = Character.Position,
            To = target,
            CollisionMask = SceneDefines.Map.CharacterLayer, //TODO make it interact with ground layer if shot from inside a trench
        });
        // hit nothing
        if (hits.Count == 0) return new FireHitResult.Miss(target);
        // hit character
        else if (hits[SceneDefines.PhysicsKey.Collider].AsGodotObject() is CharacterSimulator hitsim)
            return new FireHitResult.Hit(hits[SceneDefines.PhysicsKey.Position].AsVector2(), hitsim.Character);
        // hit something else
        return new FireHitResult.Miss(hits[SceneDefines.PhysicsKey.Position].AsVector2());
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
        CollisionMask = SceneDefines.Map.TrenchTileLayer | SceneDefines.Map.BarrierLayer;
        SetPhysicsProcess(true);
    }
}
