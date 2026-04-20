
using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Player;


/// <summary>
/// Interprets aspects of <see cref="WorldQuery"/>
/// </summary>
public static class WorldQueryInterpreter
{
    private delegate Shape2D MakeShape(Vector2 origin);
    private static Dictionary<Type, Shape2D> _intersectDictionary = new Dictionary<Type, Shape2D>()
    {
        {
            typeof(WorldQuery.MeeleeThreats), 
            new CircleShape2D()
            {
                Radius = 4 * CommonDefines.CellSize,
            }
        },
        {
            typeof(WorldQuery.Threats), 
            new CircleShape2D()
            {
                Radius = 10 * CommonDefines.CellSize,
            }
        },
        {
            typeof(WorldQuery.RangeForward), 
            new CircleShape2D()
            {
                Radius = 5 * CommonDefines.CellSize,
            }
        },
    };
    public static Shape2D GetIntersectShape(WorldQuery query)
    {

        return _intersectDictionary[query.GetType()];
    }

    public static uint GetMask(WorldQuery query)
    {
        return SceneDefines.Map.GroundObjectLayer | SceneDefines.Map.GroundTileLayer;
    }
}