using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;
/// <summary>
/// <see cref="CharacterRenderer"/> component for a client's player character.
/// </summary>
public partial class FocusCamera : Node2D
{
    /// <summary>
    /// How far the camera should see
    /// </summary>
    private const float ViewMultiplier = 1.5f;

    private Line2D _aimLine;
    private Camera2D _camera;
    private CharacterRenderer? _ownerRenderer;

    private Vector2 _viewVector;

    public void SetViewVector(Vector2 vector)
    {
        _viewVector = vector;

        float zoom = 1f / (1f + (vector.Length() / 1000f));
        _camera.Position = vector * ViewMultiplier;
        _camera.Zoom = Vector2.One * zoom;

        Vector2 mouseWorldPosition = _camera.Position + (vector / zoom);
        _aimLine.SetPointPosition(1, ClipAimToCollision(mouseWorldPosition));
    }

    private float _moveVelocity = 0;

    public FocusCamera()
    {
        _aimLine = new()
        {
            Width = 2f,
            DefaultColor = new(0.6f, 0.6f, 0.6f, 1f),
            Points = [Vector2.Zero, Vector2.Zero],
            Visible = false,
        };
        AddChild(_aimLine);

        _camera = new();
        AddChild(_camera);
        // Position = new Vector2(0, 0);
        // Zoom = new Vector2(0.4f, 0.4f);
    }

    public override void _Ready()
    {
        _ownerRenderer = GetParent() as CharacterRenderer;
        _camera.MakeCurrent();
    }

    public override void _Process(double delta)
    {
        Vector2 center = GetViewportRect().Size / 2f;

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            SetViewVector(GetViewport().GetMousePosition() - center);
            if (!_aimLine.Visible) _aimLine.Visible = true;
            // _aimLine.Points = [Vector2.Zero, Position];
        }
        else
        {
            if (_aimLine.Visible) _aimLine.Visible = false;
        }
    }

    private Vector2 ClipAimToCollision(Vector2 localTarget)
    {
        if (_ownerRenderer is null) return localTarget;

        Vector2 origin = GlobalPosition;
        Vector2 target = ToGlobal(localTarget);
        if (origin.DistanceSquaredTo(target) < 0.01f) return localTarget;

        Character owner = _ownerRenderer.Character;
        WorldLayer fireLayer = GetFireLayer(owner, localTarget);
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

    private WorldLayer GetFireLayer(Character owner, Vector2 localTarget)
    {
        WorldLayer fireLayer = owner.Layer;

        if (fireLayer == WorldLayer.Trench &&
            IsAiming(owner) &&
            GetTargetLayer(owner, localTarget) == WorldLayer.Ground)
        {
            return WorldLayer.Ground;
        }

        return fireLayer;
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
