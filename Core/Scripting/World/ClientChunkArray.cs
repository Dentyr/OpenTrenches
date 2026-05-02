using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scripting.World;

public class ClientChunkArray : IChunkArray2D<Chunk>
{
    //* Chunk array wrap
    //* 
    private ChunkArray2D<Chunk> _chunkArray = new();

    public int ChunkSizeX => _chunkArray.ChunkSizeX;
    public int ChunkSizeY => _chunkArray.ChunkSizeY;

    public Chunk this[int x, int y] => _chunkArray[x, y];

    public event Action<ChunkRecord<Chunk>>? ChunkChangedEvent
    {
        add => _chunkArray.ChunkChangedEvent += value;
        remove => _chunkArray.ChunkChangedEvent -= value;
    }


    //* Structures
    //*
    private Dictionary<int, ClientStructure> _structuresDictionary = [];

    public IReadOnlyDictionary<int, ClientStructure> StructureDict => _structuresDictionary;


    public event Action<ClientStructure>? NewStructureEvent;

    public ClientChunkArray() { }


    //* Tile changes
    //*

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTile(int x, int y, [NotNullWhen(true)] out TileType? tile)
    {
        if (_chunkArray.TryGetChunkContaining(x, y, out Chunk? chunk)) 
        {
            int chunkx = x % CommonDefines.ChunkSize;
            int chunky = y % CommonDefines.ChunkSize;
            tile = chunk[chunkx, chunky];
            return true;
        }
        tile = null!;
        return false;
    }

    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTile(int x, int y, TileType tile)
    {
        if (_chunkArray.TryGetChunkContaining(x, y, out Chunk? chunk))
        {
            chunk[x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize] = tile;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTileConstruction(int x, int y, [NotNullWhen(true)] out TileConstruction? tile)
    {
        if (_chunkArray.TryGetChunkContaining(x, y, out Chunk? chunk)) 
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
        if (_chunkArray.TryGetChunkContaining(x, y, out Chunk? chunk))
        {
            chunk.SetTileConstructionStatus(x, y, construct);
            return true;
        }
        return false;
    }



    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell)
    {
        if (_chunkArray.TryGetChunkContaining(x, y, out Chunk? chunk)) 
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

    //* Networking changes interface
    //*

    public void AddStructure(ClientStructure structure)
    {
        _structuresDictionary.Add(structure.Id, structure);
    }


    public void Execute(SetCellCommand setCell)
    {
        TrySetTile(setCell.X, setCell.Y, setCell.Tile);
    }


    /// <summary>
    /// Updates the chunk described by <paramref name="record"/>
    /// </summary>
    /// <returns>true if successful</returns>
    public bool TrySetChunk(ChunkRecord<Chunk> record)
    {
        if (_chunkArray.IsPositionInBounds(record.X, record.Y)) 
        {
            _chunkArray[record.X, record.Y] = record.Chunk;
            return true;
        }
        return false;
    }
}
