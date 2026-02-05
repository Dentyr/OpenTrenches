using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Scene.World;

public partial class AbstractChunkLayer<TChunkNode> : Node3D where TChunkNode : BaseChunkNode
{
    protected Grid2D<BaseChunkNode> Nodes { get; set; }
    protected ChunkArray2D Source { get; }

    public AbstractChunkLayer(ChunkArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunk;

        // set chunks 
        Nodes = new(CommonDefines.WorldSize, CommonDefines.WorldSize);
        foreach (BaseChunkNode node in Nodes.GetGridItems()) node?.QueueFree();
        for (int x = 0; x < ChunkGrid.SizeX; x ++) 
        {
            for (int y = 0; y < ChunkGrid.SizeY; y ++) 
            {
                BaseChunkNode node = new(ChunkGrid[x, y])
                {
                    Position = new(x * CommonDefines.ChunkSize, 0, y * CommonDefines.ChunkSize)
                };
                Nodes[x, y] = node;
                AddChild(node);
            }
        }
    }


    private void SetChunk(ChunkRecord record)
    {
        SetChunk(record.X, record.Y, record.Chunk);
    }
    private void SetChunk(int x, int y, Chunk chunk)
    {
        Nodes[x, y]?.QueueFree();
        BaseChunkNode node = new(chunk)
        {
            Position = new(x * CommonDefines.ChunkSize, 0, y * CommonDefines.ChunkSize)
        };
        Nodes[x, y] = node;
        AddChild(node);
    }
}