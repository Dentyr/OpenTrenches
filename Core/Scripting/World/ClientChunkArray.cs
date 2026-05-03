using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scene.World;

namespace OpenTrenches.Core.Scripting.World;

public class ClientChunkArray : ChunkArray2D<ClientChunk>, IChunkArray2D<ClientChunk>
{

    //* Structures
    //*
    private Dictionary<int, ClientStructure> _structuresDictionary = [];

    public IReadOnlyDictionary<int, ClientStructure> StructureDict => _structuresDictionary;


    public event Action<ClientStructure>? NewStructureEvent;

    public ClientChunkArray() : base((_, _) => new()) 
    { }


    //* Tile changes
    //*

    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell)
    {
        if (TryGetChunkContaining(x, y, out ClientChunk? chunk)) 
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
    public bool TrySetChunk(ChunkRecord<ClientChunk> record)
    {
        if (IsChunkInBounds(record.X, record.Y)) 
        {
            this[record.X, record.Y] = record.Chunk;
            return true;
        }
        return false;
    }
}
