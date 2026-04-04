using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.Scene.World;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;

public partial class RenderChunkLayer : Node2D
{
    private TileMapLayer TerrainLayer { get; }
    private ChunkArray2D Source { get; }

    public RenderChunkLayer(ChunkArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunk;


        // set chunks 
        TerrainLayer = new()
        {
            TileSet = TileSetLibrary.GrassTileSet,
        };
        AddChild(TerrainLayer);

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
    }
    private void LoadChunk(int x, int y, IChunk chunk)
    {
        int xoffset = x * CommonDefines.ChunkSize;
        int yoffset = y * CommonDefines.ChunkSize;
        List<Vector2I> Grass = new();
        List<Vector2I> Trench = new();
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
        GD.Print("Loading chunk " + x + ", " + y);
        TerrainLayer.SetCellsTerrainConnect([..Grass], 0, 0);
        GD.Print("Loaded chunk " + x + ", " + y);
    }


    private void SetChunk(ChunkRecord record)
    {
        SetChunk(record.X, record.Y, record.Chunk);
    }
    private void SetChunk(int x, int y, Chunk chunk)
    {
        Initialize(x, y, chunk);
    }

}