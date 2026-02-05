using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Scene.World;

public abstract partial class AbstractChunkLayer<TChunkNode> : Node3D where TChunkNode : BaseChunkNode
{
    protected Grid2D<TChunkNode> Nodes { get; set; }
    protected ChunkArray2D Source { get; }

    public AbstractChunkLayer(ChunkArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunk;


        // set chunks 
        Nodes = new(CommonDefines.WorldSize, CommonDefines.WorldSize);
        for (int x = 0; x < ChunkGrid.SizeX; x ++) 
        {
            for (int y = 0; y < ChunkGrid.SizeY; y ++) 
            {
                TChunkNode node = Initialize(x, y, ChunkGrid[x, y]);
                Nodes[x, y] = node;
                AddChild(node);
            }
        }
    }

    private TChunkNode Initialize(int x, int y, IChunk chunk)
    {
        TChunkNode node = _Initialize(chunk);
        node.Position = new(x * CommonDefines.ChunkSize, 0, y * CommonDefines.ChunkSize);
        return node;
    }
    protected abstract TChunkNode _Initialize(IChunk chunk);


    private void SetChunk(ChunkRecord record)
    {
        SetChunk(record.X, record.Y, record.Chunk);
    }
    private void SetChunk(int x, int y, Chunk chunk)
    {
        Nodes[x, y]?.QueueFree();
        TChunkNode node = Initialize(x, y, chunk);
        AddChild(node);
    }
}