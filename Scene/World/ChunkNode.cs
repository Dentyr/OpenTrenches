using System;
using System.Collections.ObjectModel;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.Libraries;

namespace OpenTrenches.Core.Scene.World;

/// <summary>
/// Combine multiple chunks to save resources
/// </summary>
public partial class ChunkNode : GridMap
{
    private Chunk Chunk { get; }



    public ChunkNode(Chunk Chunk) 
    {
        //* initializing properties
        MeshLibrary = Meshes.TerrainLibrary;
        CellSize = new(1, 2, 1);



        //* loading chunk

        this.Chunk = Chunk;
        for (int x = 0; x < CommonDefines.ChunkSize; x ++)
        {
            for (int y = 0; y < CommonDefines.ChunkSize; y ++)
            {
                SetTile(x, y, Chunk[x, y]);
                // SetCellItem(new(x, 0, y), 1);
            }
        }
    }

    private void SetTile(int x, int y, Tile? tile)
    {
        if (tile is null) 
        {
            SetCellItem(new(x, 0, y), 1);
            // SetCellItem(new(x, -1, y), -1);
        }
        else if (tile.Type is TileType.Trench) 
        {
            // SetCellItem(new(x, 0, y), -1);
            // SetCellItem(new(x, -1, y), 1);
        }
    }
}