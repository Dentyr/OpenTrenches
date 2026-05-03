using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

public class ServerChunkArray : ChunkArray2D<ServerChunk>, IServerChunkArray
{

    //* Structures
    //*
    private Dictionary<int, ServerStructure> _structuresDictionary = [];

    public IReadOnlyDictionary<int, ServerStructure> StructureDict => _structuresDictionary;

    private int _nextStructureId = 0;
    private int RequestNextStructureId() => _nextStructureId ++;

    public event Action<ServerStructure>? NewStructureEvent;


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
    public ServerChunkArray() : base((_, _) => new())
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
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTileConstruction(int x, int y, [NotNullWhen(true)] out TileConstruction? tile)
    {
        if (TryGetChunkContaining(x, y, out ServerChunk? chunk)) 
        {
            int chunkx = x % CommonDefines.ChunkSize;
            int chunky = y % CommonDefines.ChunkSize;
            if (chunk.GetTileConstructionStatus(chunkx, chunky) is TileConstruction construction)
            {
                tile = construction;
                return true;
            }
            tile = null;
            return false;
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTileConstruction(int x, int y, TileConstruction construct)
    {
        if (TryGetChunkContaining(x, y, out ServerChunk? chunk))
        {
            int chunkx = x % CommonDefines.ChunkSize;
            int chunky = y % CommonDefines.ChunkSize;
            chunk.SetTileConstructionStatus(chunkx, chunky, construct);
            return true;
        }
        return false;
    }



    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell)
    {
        if (TryGetChunkContaining(x, y, out ServerChunk? chunk)) 
        {
            int chunkx = x % CommonDefines.ChunkSize;
            int chunky = y % CommonDefines.ChunkSize;
            cell = new(
                chunk[chunkx, chunky],
                chunk.GetTileConstructionStatus(chunkx, chunky)
            );
            return true;
        }
        cell = null;
        return false;
    }

    //*

    public IEnumerable<ChunkRecord<ServerChunk>> GetChunks()
    {
        for (byte x = 0; x < ChunkSizeX; x ++) for (byte y = 0; y < ChunkSizeY; y ++) yield return new ChunkRecord<ServerChunk>(this[x, y], x, y);
    }

    private bool IsAreaInBounds(Rect2I area) 
        => IsAreaInBounds(area.Position.X, area.Position.Y, area.End.X, area.End.Y);

    //* Server functions
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
        if (TryGetChunkContaining(x, y, out ServerChunk? chunk))
        {
            chunk.SetTileConstructionStatus(x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize, new(buildTarget, initialProgress, 0));

        }
    }
    public void StartBuild(Vector2I buildCell, TileType buildTarget, float initialProgress = 0) => SetBuildTarget(buildCell.X, buildCell.Y, buildTarget, initialProgress);

    //* server structure build
    //*

    private ServerStructure MakeNewStructure(StructureType type, Team team, Vector2I position)
    {
        ServerStructure structure = new(RequestNextStructureId(), team, type, position);
        _structuresDictionary.Add(structure.Id, structure);

        // note id in chunk if exists
        if (TryGetChunkContaining(position.X, position.Y, out var chunk))
            chunk.AddStructureId(structure.Id);

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




    public void Process()
    {
        for (int x = 0; x < ChunkSizeX; x ++)
        {
            for (int y = 0; y < ChunkSizeY; y ++)
            {
                var chunk = this[x, y];
                foreach (var construct in chunk.TileConstructions)
                {
                    //TODO convert constructions into tiles
                }
            }
        }
    }
}