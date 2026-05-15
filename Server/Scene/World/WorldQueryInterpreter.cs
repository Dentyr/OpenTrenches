
using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Server.Scene;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Player;


/// <summary>
/// Interprets aspects of <see cref="WorldQuery"/> to engine specs like godot shapes.
/// </summary>
public static class WorldQueryInterpreter
{
    //TODO? maybe convert to something like Equipment type
    private delegate Shape2D MakeShape(Vector2 origin);
    private static Dictionary<WorldQuery.Shape, Shape2D> _intersectDictionary = new Dictionary<WorldQuery.Shape, Shape2D>()
    {
        {
            WorldQuery.Shape.Meelee, 
            new CircleShape2D()
            {
                Radius = 4 * CommonDefines.CellSize,
            }
        },
        {
            WorldQuery.Shape.Range, 
            new CircleShape2D()
            {
                Radius = 10 * CommonDefines.CellSize,
            }
        },
        {
            WorldQuery.Shape.Distance, 
            new CircleShape2D()
            {
                Radius = 5 * CommonDefines.CellSize,
            }
        },
        {
            WorldQuery.Shape.Area,
            new RectangleShape2D()
            {
                Size = new Vector2(CommonDefines.AreaSize, CommonDefines.AreaSize) * CommonDefines.CellSize
            }
        },
    };
    public static Shape2D GetIntersectShape(WorldQuery query)
    {

        return _intersectDictionary[query.QueryArea];
    }

    public static uint GetMask(WorldQuery query)
    {
        //TODO take into account structures if wanted
        return SceneDefines.Map.CharacterLayer;
    }
}
