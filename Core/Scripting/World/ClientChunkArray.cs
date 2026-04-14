using System;
using System.Collections.Generic;
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

    public int SizeX => _chunkArray.SizeX;
    public int SizeY => _chunkArray.SizeY;

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

    public bool TryGetTile(int x, int y, out Tile? tile) => _chunkArray.TryGetTile(x, y, out tile);
    public bool TryGetTile(Vector2I cell, out Tile? tile) => TryGetTile(cell.X, cell.Y, out tile);

    public bool TrySetTile(int x, int y, Tile? tile) => _chunkArray.TrySetTile(x, y, tile);

    //* Networking changes interface
    //*

    public void AddStructure(ClientStructure structure)
    {
        _structuresDictionary.Add(structure.Id, structure);
    }


    public void Execute(SetCellCommand setCell)
    {
        TrySetTile(setCell.CellRecord.X, setCell.CellRecord.Y, setCell.CellRecord.TileRecord is null ? null : CommonFromDTO.Convert(setCell.CellRecord.TileRecord));
    }
    public void SetChunk(ChunkRecord record) => _chunkArray.SetChunk(record);
}
