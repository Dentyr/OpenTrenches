using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scene.World;

namespace OpenTrenches.Core.Scripting.World;

public class ClientChunkArray : ChunkArray2D, ITileArray2D
{

    //* Structures
    //*
    private Dictionary<int, ClientStructure> _structuresDictionary = [];

    public IReadOnlyDictionary<int, ClientStructure> StructureDict => _structuresDictionary;


    public event Action<ClientStructure>? NewStructureEvent;

    public ClientChunkArray()
    { }



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
    /// Bulk sets <paramref name="gridmap"/> tiles at <paramref name="xOffset"/> <paramref name="yOffset"/>
    /// </summary>
    /// <param name="gridmap"></param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    public void Set(TileType[][] gridmap, int xOffset, int yOffset)
    {
        // int maxX = Math.Min(gridmap.Length + xOffset, SizeX);
        for (int x = 0; x < gridmap.Length; x ++)
        {
            for (int y = 0; y < gridmap.Length; y ++)
            {
                this[x + xOffset, y + yOffset] = gridmap[x][y];
            }
        }
    }
}
