using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

public class ServerChunkArray : ChunkArray2D, IServerChunkArray
{

    //* Structures
    //*
    private Dictionary<int, ServerStructure> _structuresDictionary = [];

    public IReadOnlyDictionary<int, ServerStructure> StructureDict => _structuresDictionary;

    private int _nextStructureId = 0;
    private int RequestNextStructureId() => _nextStructureId ++;

    public event Action<ServerStructure>? NewStructureEvent;


    /// <summary>
    /// Tiles being updated
    /// </summary>
    private Dictionary<Vector2I, TileConstruction> _tileConstructions = [];
    public IReadOnlyDictionary<Vector2I, TileConstruction> TileConstructions => _tileConstructions;

    /// <summary>
    /// Called when the tile construction at the local position (x, y) is set or removed
    /// </summary>
    public event Action<TileConstruction?, int, int>? TileConstructionSet;

    //* Updates
    //* 

    /// <summary>
    /// Infrequent state changes, such as type of tile
    /// </summary>
    private PolledQueue<SetCellCommand> TileChanges { get; } = new();
    public IEnumerable<SetCellCommand> PollCellChanges() => TileChanges.PollItems();

    /// <summary>
    /// frequent state changes, such as build progress
    /// </summary>
    private PolledQueue<WorldGridAttributeUpdateDTO> Updates { get; } = new();

    
    /// <summary>
    /// Initializes empty chunks
    /// </summary>
    public ServerChunkArray()
    {}

    //* Tile changes
    //*

    /// <summary>
    /// Adds tile changes to outgoing network queue
    /// </summary>
    protected override void _OnTileSet(int x, int y, TileType tile)
    {
        TileChanges.Enqueue(new SetCellCommand(x, y, tile));
    }


    /// <summary>
    /// Returns true if <paramref name="cell"/> exists with ongoing construction, returning it in <paramref name="construct"/>.
    /// </summary>
    public bool TryGetTileConstruction(int x, int y, [NotNullWhen(true)] out TileConstruction? construct)
    {
        return TileConstructions.TryGetValue(new(x, y), out construct);
    }
    /// <summary>
    /// Returns construction in <paramref name="cell"/> or null if it doesn't exist
    /// </summary>
    public TileConstruction? GetTileConstructionOrDefault(int x, int y)
    {
        return TileConstructions.GetValueOrDefault(new(x, y));
    }


    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTileConstruction(int x, int y, TileConstruction construct)
    {
        if (IsPositionInBounds(x, y))
        {
            _tileConstructions[new(x, y)] = construct;
            TileConstructionSet?.Invoke(construct, x, y);
            return true;
        }
        return false;
    }

    public IEnumerable<TileConstructionRecord> GetActiveTileConstruction()
    {
        return _tileConstructions.Select(
            kvp => new TileConstructionRecord(kvp.Key.X, kvp.Key.Y, kvp.Value)
        );
    }


    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell)
    {
        if (IsPositionInBounds(x, y)) 
        {
            cell = new(
                this[x, y],
                GetTileConstructionOrDefault(x, y)
            );
            return true;
        }
        cell = null;
        return false;
    }



    //* Server tile build
    //*

    /// <summary>
    /// Increases building progress for the cell at (<paramref name="x"/>, <paramref name="y"/>). Returns true if completed building or cannot build, and false if it is still progressing build. 
    /// </summary>
    public TileConstruction? ProgressBuild(int x, int y, float progress)
    {
        if (TryGetTileConstruction(x, y, out var status))
        {
            // throw new Exception();
            status.Progress += progress;
            if (status.Progress > 1)
            {
                TrySetTile(x, y, status.Target);
            }
            else 
            {
                Updates.Enqueue(WorldGridAttributeUpdateDTO.CreateBuildProgress(x, y, status.Progress));
            }
        }
        return status;
    }
    public TileConstruction? ProgressBuild(Vector2I buildCell, float progress) => ProgressBuild(buildCell.X, buildCell.Y, progress);

    /// <summary>
    /// Attempts to start converting the tile at (<paramref name="x"/>, <paramref name="y"/>) into <paramref name="buildTarget"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="buildTarget"></param>
    /// <param name="initialProgress"></param>
    public void SetBuildTarget(int x, int y, TileType buildTarget, float initialProgress)
    {
        if (IsPositionInBounds(x, y))
            _tileConstructions[new(x, y)] = new(buildTarget, initialProgress, 0);
    }
    public void StartBuild(Vector2I buildCell, TileType buildTarget, float initialProgress = 0) 
        => SetBuildTarget(buildCell.X, buildCell.Y, buildTarget, initialProgress);


    //* server structure build
    //*

    private ServerStructure MakeNewStructure(StructureType type, Team team, Vector2I position)
    {
        ServerStructure structure = new(RequestNextStructureId(), team, type, position);
        _structuresDictionary.Add(structure.Id, structure);

        NewStructureEvent?.Invoke(structure);
        return structure;
    }

    public bool TryBuild(Vector2I buildCell, Team team, StructureEnum structure, out ServerStructure? buildResult)
    {
        StructureType type = StructureTypes.Get(structure);

        Rect2I area = type.Profile.Translate(buildCell);
        if (IsAreaInBounds(area) && !IsSpaceOccupied(area))
        {
            buildResult = MakeNewStructure(type, team, buildCell);
            return true;
        }
        buildResult = null;
        return false;
    }

    //* geometry

    private bool IsAreaInBounds(Rect2I area) 
        => IsAreaInBounds(area.Position.X, area.Position.Y, area.End.X, area.End.Y);

    /// <summary>
    /// Checks if the spaces in the rect2I overlap with any structure
    /// </summary>
    private bool IsSpaceOccupied(Rect2I space)
    {
        //TODO optimize only check nearby chunks
        foreach(ServerStructure structure in StructureDict.Values)
            if (structure.GetProfile().Intersects(space)) return true;
        return false;
    }
    /// <summary>
    /// Checks if <paramref name="cell"/> is occupied by any structure
    /// </summary>
    private bool IsSpaceOccupied(Vector2I cell)
    {
        //TODO optimize only check nearby chunks
        foreach(ServerStructure structure in StructureDict.Values)
            if (structure.GetProfile().HasPoint(cell)) return true;
        return false;
    }


    //* processing

    public void Process()
    {
        foreach (var construct in TileConstructions)
        {
            //TODO convert constructions into tiles
        }
    }

}