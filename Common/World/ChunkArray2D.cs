using System;
using System.Collections;
using System.Collections.Generic;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

public class ChunkArray2D
{
    private Grid2D<Chunk> Chunks { get; } = new(CommonDefines.WorldSize, CommonDefines.WorldSize, () => new Chunk());

    public int SizeX => Chunks.SizeX;
    public int SizeY => Chunks.SizeY;

    public Chunk this[int x, int y]
    {
        get => Chunks[x, y];
        set => Chunks[x, y] = value;
    }

    public ChunkArray2D() 
    {

    }

    public IEnumerable<ChunkRecord> GetChunks()
    {
        for (byte x = 0; x < SizeX; x ++) for (byte y = 0; y < SizeY; y ++) yield return new ChunkRecord(this[x, y], x, y);
    }
}