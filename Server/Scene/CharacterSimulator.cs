using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;
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

    public CharacterSimulator(Character Character)
    {
        //* character
        this.Character = Character;
        // convert local position to cell location
        SyncPosition();
        
        Character.DiedEvent += Deactivate;
        Character.RespawnEvent += Activate;
        Character.LayerChangedEvent += SetCollisionLayer;

        //* collision
        AddChild(new CollisionShape2D()
        {
            Shape = new CircleShape2D()
            {
                Radius = 12,
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
        // move from logical space to engine space
        target *= CommonDefines.CellSize;
        var hits = GetViewport().World2D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
            From = Position,
            To = target,
            CollisionMask = SceneDefines.Map.GroundObjectLayer | SceneDefines.Map.BarrierLayer, //TODO make it interact with ground layer if shot from inside a trench
        });
        // hit nothing
        if (hits.Count == 0) return new FireHitResult.Miss(target);
        else
        {
            //if hit something
            GodotObject hitObject = hits[SceneDefines.PhysicsKey.Collider].AsGodotObject();

            // Hit position in logical space;
            var hitPos = hits[SceneDefines.PhysicsKey.Position].AsVector2() / CommonDefines.CellSize;

            // hit character
            if (hitObject is CharacterSimulator hitsim)
                return new FireHitResult.Hit(hitPos, hitsim.Character);
            // hit something else
            else 
                return new FireHitResult.Miss(hitPos);
        }
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
        SetPhysicsProcess(true);
        SetCollisionLayer(Character.Layer);
    }

    /// <summary>
    /// Sets collision layer and mask to the appropriate values, if processing physics
    /// </summary>
    /// <param name="layer"></param>
    private void SetCollisionLayer(WorldLayer layer)
    {
        if (!IsPhysicsProcessing()) return;
        switch (layer)
        {
            case WorldLayer.Ground:
                CollisionLayer = SceneDefines.Map.GroundObjectLayer;
                CollisionMask = SceneDefines.Map.BarrierLayer;
                break;
            case WorldLayer.Trench:
                CollisionLayer = SceneDefines.Map.TrenchObjectLayer;
                CollisionMask = SceneDefines.Map.GroundTileLayer | SceneDefines.Map.BarrierLayer;
                break;
        }
    }
}
