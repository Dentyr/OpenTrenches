using System;
using System.Collections.ObjectModel;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Scene.World;

/// <summary>
/// Combine multiple chunks to save resources
/// </summary>
public partial class BaseChunkNode : GridMap
{
    protected IChunk Chunk { get; }

    public virtual Tile? this[int x, int y]
    {
        get => Chunk[x, y];
        set
        {
            if (value is null) 
            {
                SetCellItem(new(x, 0, y), Meshes.Terrain.GrassIdx);
                // SetCellItem(new(x, -1, y), -1);
            }
            else if (value.Type is TileType.Trench) 
            {
                SetCellItem(new(x, 0, y), -1);
                SetCellItem(new(x, -1, y), Meshes.Terrain.TrenchIdx);
            }
        }
    }



    public BaseChunkNode(IChunk Chunk) 
    {
        //* initializing properties
        MeshLibrary = Meshes.Terrain.Library;
        CellSize = new(CommonDefines.CellSize, 2, CommonDefines.CellSize);



        //* loading chunk

        this.Chunk = Chunk;
        for (int x = 0; x < CommonDefines.ChunkSize; x ++)
        {
            for (int y = 0; y < CommonDefines.ChunkSize; y ++)
            {
                this[x, y] = Chunk[x, y];
                // SetCellItem(new(x, 0, y), 1);
            }
        }

        //* subscribing to events
        this.Chunk.TerrainChangeEvent += HandleTerrainChange;
        // HandleTerrainChange(new Tile(TileType.Trench, 100, null), 10, 10);
    }
    // private void HandleTerrainChange(Tile? tile, int x, int y) => this[x, y] = new Tile(TileType.Trench, 100, null);
    private void HandleTerrainChange(Tile? tile, int x, int y) => this[x, y] = tile;
    // private void HandleTerrainChange(Tile? tile, int x, int y)
    // {
    //     for (int i = 0; i < CommonDefines.ChunkSize; i ++)
    //     {
    //         for (int j = 0; j < CommonDefines.ChunkSize; j ++)
    //         {
    //             this[i, j] = tile;
    //             // SetCellItem(new(x, 0, y), 1);
    //         }
    //     }
    // }

}