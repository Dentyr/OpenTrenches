using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Scene.World;

public partial class ChunkLayer : Node2D
{
    private readonly TileMapLayer TerrainLayer;
    private readonly TileMapLayer WallLayer;

    protected ChunkArray2D Source { get; }

    public ChunkLayer(ChunkArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunk;


        // set chunks 
        TerrainLayer = new()
        {
            TileSet = TileSetLibrary.GrassTileSet,
        };
        AddChild(TerrainLayer);

        WallLayer = new()
        {
            TileSet = TileSetLibrary.WallTileSet,
        };
        AddChild(WallLayer);

        for (int x = 0; x < ChunkGrid.SizeX; x ++) 
        {
            for (int y = 0; y < ChunkGrid.SizeY; y ++) 
            {
                Initialize(x, y, ChunkGrid[x, y]);
            }
        }


    }

    private void Initialize(int x, int y, IChunk chunk)
    {
        LoadChunk(x, y, chunk);
        chunk.TerrainChangeEvent += UpdateTile;
    }

    private void UpdateTile(Tile? tile, int x, int y)
    {
        
        TerrainLayer.SetCellsTerrainConnect([new(x, y)], GetTerrainFromTile(tile?.Type), 0);
    }

    private void LoadChunk(int x, int y, IChunk chunk)
    {
        int xoffset = x * CommonDefines.ChunkSize;
        int yoffset = y * CommonDefines.ChunkSize;
        List<Vector2I> Grass = [];
        List<Vector2I> Trench = [];
        for (int cellx = 0; cellx < CommonDefines.ChunkSize; cellx ++)
        {
            for (int celly = 0; celly < CommonDefines.ChunkSize; celly ++)
            {
                Vector2I position = new(xoffset + cellx, yoffset + celly);
                switch (chunk[x, y]?.Type)
                {
                    case null:
                    case TileType.Clear:
                        Grass.Add(position);
                        break;
                    case TileType.Trench:
                        Trench.Add(position);
                        break;
                }
            }
        }
        TerrainLayer.SetCellsTerrainConnect([..Grass], 0, 0);
    }
    private int GetTerrainFromTile(TileType? type)
    {
        switch (type)
        {
            default:
            case null:
            case TileType.Clear:
                return 0;
            case TileType.Trench:
                return 1;
        }
    }


    private void SetChunk(ChunkRecord record)
    {
        SetChunk(record.X, record.Y, record.Chunk);
    }
    private void SetChunk(int x, int y, Chunk chunk)
    {
        LoadChunk(x, y, chunk);
    }

}