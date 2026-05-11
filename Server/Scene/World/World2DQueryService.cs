using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Util;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scene.World;

/// <summary>
/// Godot implementation of <see cref="IWorld2DQueryService"/>
/// </summary>
public class World2DQueryService : IWorld2DQueryService
{
    private readonly PhysicsDirectSpaceState2D _state;
    public World2DQueryService(PhysicsDirectSpaceState2D state)
    {
        _state = state;
    }
    /// <summary>
    /// Uses <paramref name="query"/> to search for certain objects in the world such as characters and structures
    /// </summary>
    /// <param name="query"></param>
    public WorldQueryResult Query(QueryContext context, WorldQuery query)
    {
        var worldPosition = context.Position * CommonDefines.CellSize;
        // get correct shape
        var hits = _state.IntersectShape(new PhysicsShapeQueryParameters2D()
        {
            Transform = GeometryServices.MakeTranslate(worldPosition),
            Shape = WorldQueryInterpreter.GetIntersectShape(query),
            CollisionMask = WorldQueryInterpreter.GetMask(query)
        });
        // Get intersect
        List<Character> characters = [];
        List<ServerStructure> structures = [];

        foreach (var hit in hits)
        {
            GodotObject hitobj = hit[SceneDefines.PhysicsKey.Collider].AsGodotObject();
            // hit valid
            if (hitobj is CharacterSimulator charaSim)
            {
                if ((charaSim.Character.Team == context.Team && query.IncludeAlly) ||
                    (charaSim.Character.Team != context.Team && query.IncludeEnemy)
                )
                {
                    characters.Add(charaSim.Character);
                }
            }
            else if (hitobj is StructureSimulator structSim)
            {
                if ((structSim.Structure.Team == context.Team && query.IncludeAlly) ||
                    (structSim.Structure.Team != context.Team && query.IncludeEnemy)
                )
                {
                    structures.Add(structSim.Structure);
                }
            }
        }

        return new WorldQueryResult(
            characters,
            structures
        );
    }
}
