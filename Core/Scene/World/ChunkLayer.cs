using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;

public partial class ChunkLayer : Node3D
{
    private Grid2D<ChunkNode?> Chunks { get; set; } = new(CommonDefines.WorldSize, CommonDefines.WorldSize);
    
    public ChunkLayer()
    {
        
    }
    // public void SetChunks(ChunkArray2D chunkGrid)
    // {
    //     Chunks.Fill(null);
    //     for (int x = 0; x < chunkGrid.SizeX; x ++) 
    //     {
    //         for (int y = 0; y < chunkGrid.SizeY; y ++) 
    //         {
    //             ChunkNode node = new(chunkGrid[x, y]);
    //             Chunks[x, y] = node;
    //             node.Position = new(x * CommonDefines.ChunkSize, 0, y * CommonDefines.ChunkSize);
    //             AddChild(node);
    //         }
    //     }
    // }
    public void SetChunk(ChunkRecord record)
    {
        Chunks[record.X, record.Y]?.QueueFree();
        ChunkNode node = new(record.Chunk);
        node.Position = new(record.X * CommonDefines.ChunkSize, 0, record.Y * CommonDefines.ChunkSize);
        Chunks[record.X, record.Y] = node;
        AddChild(node);
    }
}