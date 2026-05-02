using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scripting.World;

public class ClientChunkArray : IChunkArray2D
{
    //* Chunk array wrap
    //* 
    private ChunkArray2D _chunkArray = new();

    public int ChunkSizeX => _chunkArray.ChunkSizeX;
    public int ChunkSizeY => _chunkArray.ChunkSizeY;

    public Chunk this[int x, int y] => _chunkArray[x, y];

    public event Action<ChunkRecord>? ChunkChangedEvent
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

    public bool TryGetTile(int x, int y, [NotNullWhen(true)] out TileType? tile) => _chunkArray.TryGetTile(x, y, out tile);
    public bool TryGetTile(Vector2I cell, [NotNullWhen(true)] out TileType? tile) => TryGetTile(cell.X, cell.Y, out tile);

    public bool TrySetTile(int x, int y, TileType tile) => _chunkArray.TrySetTile(x, y, tile);

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
    public void SetChunk(ChunkRecord record) => _chunkArray.SetChunk(record);

    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell)
        => _chunkArray.TryGetCell(x, y, out cell);
}
