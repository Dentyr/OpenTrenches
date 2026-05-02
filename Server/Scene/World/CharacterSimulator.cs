using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Util;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Combat;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.World;

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
        Character.LayerChangedEvent += SetMovementMask;

        //* collision
        AddChild(new CollisionShape2D()
        {
            Shape = new CircleShape2D()
            {
                Radius = CommonDefines.CharacterRadius,
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

    FireHitResult ICharacterAdapter.AdaptFire(WorldLayer channel, Vector2 target)
    {
        // move from logical space to engine space
        target *= CommonDefines.CellSize;

        Godot.Collections.Array<Rid> exclude = [];

        while ( true )
        {
            var hits = GetViewport().World2D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
            {
                From = Position,
                To = target,
                CollisionMask = PhysicsLayerInterpreter.GetScanLayer(channel),
                Exclude = exclude,
            });

            // hit nothing
            if (hits.Count == 0) return new FireHitResult.Miss(target);
            else
            {
                //if hit something
                GodotObject hitObject = hits[SceneDefines.PhysicsKey.Collider].AsGodotObject();

                // Hit position in logical space;
                var hitPos = hits[SceneDefines.PhysicsKey.Position].AsVector2() / CommonDefines.CellSize;

                // hit valid
                if (hitObject is CharacterSimulator charaSim)
                {
                    if (CombatCalculationService.TryHit(channel, charaSim.Character))
                        return new FireHitResult.HitCharacter(hitPos, charaSim.Character);
                    exclude.Add(hits[SceneDefines.PhysicsKey.Rid].AsRid());
                }
                else if (hitObject is StructureSimulator structSim)
                {
                    return new FireHitResult.HitStructure(hitPos, structSim.Structure);
                }
                // hit something else
                else 
                    return new FireHitResult.Miss(hitPos);
            }
        }
    }
    
    WorldQueryResult ICharacterAdapter.Query(WorldQuery query)
    {
        // get correct shape
        var hits = GetViewport().World2D.DirectSpaceState.IntersectShape(new PhysicsShapeQueryParameters2D()
        {
            Transform = GeometryServices.MakeTranslate(Position),
            Shape = WorldQueryInterpreter.GetIntersectShape(query),
            CollisionMask = WorldQueryInterpreter.GetMask(query)
        });
        // Get intersect
        List<Character> characters = [];
        List<ServerStructure> structures = [];

        foreach(var hit in hits)
        {
            GodotObject hitobj = hit[SceneDefines.PhysicsKey.Collider].AsGodotObject();
            // hit valid
            if (hitobj is CharacterSimulator charaSim)
            {
                if ((charaSim.Character.Team == Character.Team && query.IncludeAlly) ||
                    (charaSim.Character.Team != Character.Team && query.IncludeEnemy)
                ) {
                    characters.Add(charaSim.Character);
                }
            }
            else if (hitobj is StructureSimulator structSim)
            {
                if ((structSim.Structure.Team == Character.Team && query.IncludeAlly) ||
                    (structSim.Structure.Team != Character.Team && query.IncludeEnemy)
                ) {
                    structures.Add(structSim.Structure);
                }
            }
        }

        return new WorldQueryResult(
            characters,
            structures
        );
    }

    /// <summary>
    /// Checks for a ground tile nearby in <paramref name="direction"/>, and if unoccupied, returns the position in game position
    /// </summary>
    /// <returns></returns>
    public Vector2? AdaptJump(Vector2 direction)
    {
        direction = direction.Normalized();
        var target = Position + direction * CommonDefines.CellSize / 2;

        // find nearest ground tile
        var hits = GetViewport().World2D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
            From = Position,
            To = target,
            CollisionMask = SceneDefines.Map.GroundTileLayer,
        });
        if (hits.Count == 0) return null; //could not find a nearby ledge

        Vector2 normal = hits[SceneDefines.PhysicsKey.Normal].AsVector2();
        Vector2 hitlocation = hits[SceneDefines.PhysicsKey.Position].AsVector2();

        // target position to move to
        Vector2 targetPosition = hitlocation - (normal * CommonDefines.CellSize / 2);

        // ensure space is unoccupied
        var occupationHits = GetViewport().World2D.DirectSpaceState.IntersectShape(new PhysicsShapeQueryParameters2D()
        {
            Transform = GeometryServices.MakeTranslate(targetPosition),
            Shape = new CircleShape2D() { Radius = CommonDefines.CharacterRadius }, // TODO make runtime constant
            // Modify for collision later
            CollisionMask = SceneDefines.Map.BarrierLayer,
        });

        if (occupationHits.Count > 0) return null;
        
        // convert back to world position
        return targetPosition / CommonDefines.CellSize;
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
        CollisionLayer = SceneDefines.Map.CharacterLayer;;
        SetMovementMask(Character.Layer);
    }

    /// <summary>
    /// Sets collision layer and mask to the appropriate values, if processing physics
    /// </summary>
    /// <param name="layer"></param>
    private void SetMovementMask(WorldLayer layer)
    {
        if (!IsPhysicsProcessing()) return;
        switch (layer)
        {
            case WorldLayer.Ground:
                CollisionMask = SceneDefines.Map.BarrierLayer;
                break;
            case WorldLayer.Trench:
                CollisionMask = SceneDefines.Map.GroundTileLayer | SceneDefines.Map.BarrierLayer;
                break;
        }
    }

    // private void HandleStateChange(CharacterState state)
    // {
    //     //? Maybe change to have individual events fire for specific flag clearing and setting instead of having a single point for state changes
    //     UpdateCollisionAiming(state.HasFlag(CharacterState.Aiming));
    // }
    // /// <summary>
    // /// Modifies collision layer to be hittable by ground fire if the character is aiming out of a trench
    // /// </summary>
    // private void UpdateCollisionAiming(bool isAiming)
    // {
    //     //? check whether it needs updating first if performance is issue

    //     // if aiming out of trench, it should be able to be hit by ground targets
    //     if (isAiming)
    //         CollisionLayer |= SceneDefines.Map.GroundTileLayer;
    //     // otherwise if it's inside a trench then it should be protected
    //     else if (Character.MovementLayer == WorldLayer.Trench)
    //         CollisionLayer &= ~SceneDefines.Map.GroundTileLayer;
    // }
}
