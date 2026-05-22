using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;
/// <summary>
/// Manages player aim and associated zoom/indicators
/// </summary>
public partial class PlayerAimController : Node2D
{
    /// <summary>
    /// How far the camera should see
    /// </summary>
    private const float ViewMultiplier = 1.5f;
    private const float BaseMoveVelocity = 100f;
    private const float MoveVelocityDistanceFactor = 10f;

    private CharacterRenderer? _ownerRenderer;


    private Camera2D _camera;
    private Cursor _cursor;
    private Line2D _aimLine;

    private Vector2 _viewVector;
    private Vector2 _targetVector;

    private float _moveVelocity = 0;

    private readonly IReadOnlyPlayerState _playerState;

    public PlayerAimController(IReadOnlyPlayerState state)
    {
        _playerState = state;

        _aimLine = new()
        {
            Width = 2f,
            DefaultColor = new(0.6f, 0.6f, 0.6f, 1f),
            Points = [Vector2.Zero, Vector2.Zero],
            Visible = false,
        };
        AddChild(_aimLine);

        _cursor = new();
        AddChild(_cursor);

        _camera = new();
        AddChild(_camera);
    }


    private void SetViewVector(Vector2 vector)
    {
        _viewVector = vector;

        float zoom = 1f / (1f + (vector.Length() / 1000f));
        _camera.Position = vector * ViewMultiplier;
        _camera.Zoom = Vector2.One * zoom;

        _cursor.Position = vector * ViewMultiplier;

        Vector2 mouseWorldPosition = _camera.Position + (vector / zoom);
        _aimLine.SetPointPosition(1, ClipAimToCollision(mouseWorldPosition));
    }


    public override void _Ready()
    {
        _ownerRenderer = GetParent() as CharacterRenderer;
        _camera.MakeCurrent();
    }

    public override void _Process(double delta)
    {
        Vector2 center = GetViewportRect().Size / 2f;

        if (_playerState.PrimarySlotState is not null)
            _cursor.SetRecoil(_playerState.PrimarySlotState.Recoil);


        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            _targetVector = GetViewport().GetMousePosition() - center;
            if (!_aimLine.Visible) _aimLine.Visible = true;
            if (!_cursor.Visible) _cursor.Visible = true;
            // _aimLine.Points = [Vector2.Zero, Position];
        }
        else
        {
            _targetVector = new(0, 0);
            if (_aimLine.Visible) _aimLine.Visible = false;
            if (_cursor.Visible) _cursor.Visible = false;
        }

        if (_viewVector != _targetVector)
        {
            var difference = _targetVector - _viewVector;
            float distLeft = difference.Length();
            float distToTraverse = (BaseMoveVelocity + (distLeft * MoveVelocityDistanceFactor)) * (float)delta;

            if (distLeft <= distToTraverse)
                SetViewVector(_targetVector);
            else 
                SetViewVector(_viewVector + (difference.Normalized() * distToTraverse));
        }
    }

    private Vector2 ClipAimToCollision(Vector2 localTarget)
    {
        if (_ownerRenderer is null) return localTarget;

        Vector2 origin = GlobalPosition;
        Vector2 target = ToGlobal(localTarget);
        if (origin.DistanceSquaredTo(target) < 0.01f) return localTarget;

        Character owner = _ownerRenderer.Character;
        WorldLayer fireLayer = GetFireLayer(owner, origin, target);
        uint collisionMask = GetScanLayer(fireLayer);
        Godot.Collections.Array<Rid> exclude = [_ownerRenderer.GetRid()];

        while (true)
        {
            Godot.Collections.Dictionary hit = GetViewport().World2D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
            {
                From = origin,
                To = target,
                CollisionMask = collisionMask,
                Exclude = exclude,
                CollideWithAreas = true,
                CollideWithBodies = true,
            });

            if (hit.Count == 0) return localTarget;

            GodotObject hitObject = hit[PhysicsDefines.PhysicsKey.Collider].AsGodotObject();
            if (hitObject is CharacterRenderer characterRenderer &&
                !CanHitCharacter(fireLayer, owner, characterRenderer.Character))
            {
                exclude.Add(hit[PhysicsDefines.PhysicsKey.Rid].AsRid());
                continue;
            }

            return ToLocal(hit[PhysicsDefines.PhysicsKey.Position].AsVector2());
        }
    }

    /// <summary>
    /// Returns the layer for <paramref name="owner"/> to shoot into if they are aiming from <paramref name="origin"/> to <paramref name="target"/> in engine space
    /// </summary>
    private WorldLayer GetFireLayer(Character owner, Vector2 origin, Vector2 target)
    {
        WorldLayer fireLayer = owner.Layer;

        // if owner is aiming from a trench, then if their destination is a ground tile or is impeded by a ground tile they will shoot out of the trench
        if (fireLayer == WorldLayer.Trench &&
            IsAiming(owner) &&
            LineIntersectsGround(origin, target)
        ) {
            return WorldLayer.Ground;
        }

        return fireLayer;
    }
    private bool LineIntersectsGround(Vector2 origin, Vector2 target)
    {
        Godot.Collections.Dictionary hit = GetViewport().World2D.DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
            From = origin,
            To = target,
            CollisionMask = PhysicsDefines.Map.GroundTileLayer,
            CollideWithAreas = true,
            CollideWithBodies = true,
        });

        return hit.Count > 0;
    }

    private bool IsAiming(Character owner)
        => Input.IsMouseButtonPressed(MouseButton.Right) ||
            owner.ActionState.HasFlag(CharacterState.Aiming);

    private WorldLayer GetTargetLayer(Character owner, Vector2 localTarget)
    {
        Vector2 targetCell = ToGlobal(localTarget) / CommonDefines.CellSize;
        if (owner.ClientState.Chunks.TryGetTile((int)targetCell.X, (int)targetCell.Y, out TileType? tile))
        {
            return TileLayerConversion.LayerOf(tile);
        }

        return WorldLayer.Ground;
    }

    private static uint GetScanLayer(WorldLayer channel)
    {
        return channel switch
        {
            WorldLayer.Trench => PhysicsDefines.Map.StructureLayer |
                PhysicsDefines.Map.BarrierLayer |
                PhysicsDefines.Map.CharacterLayer |
                PhysicsDefines.Map.GroundTileLayer,
            _ => PhysicsDefines.Map.StructureLayer |
                PhysicsDefines.Map.BarrierLayer |
                PhysicsDefines.Map.CharacterLayer,
        };
    }

    private static bool CanHitCharacter(WorldLayer fireLayer, Character owner, Character target)
    {
        if (!target.IsActive || owner.Equals(target)) return false;
        if (fireLayer == target.Layer) return true;

        return fireLayer == WorldLayer.Ground &&
            target.Layer == WorldLayer.Trench &&
            target.ActionState.HasFlag(CharacterState.Aiming);
    }
}
