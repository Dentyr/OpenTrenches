using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Adapter;

namespace OpenTrenches.Common.World;

public class ChunkArray2D : IChunkArray2D
{
    private Grid2D<Chunk> Chunks { get; } = new(CommonDefines.WorldSize, CommonDefines.WorldSize, (_, _) => new Chunk());

    public int SizeX => Chunks.SizeX;
    public int SizeY => Chunks.SizeY;

    public Chunk this[int x, int y]
    {
        get => Chunks[x, y];
    }

    /// <summary>
    /// Infrequent state changes, such as type of tile
    /// </summary>
    private PolledQueue<SetCellCommand> TileChanges { get; } = new();
    public IEnumerable<SetCellCommand> PollCellChanges() => TileChanges.PollItems();

    /// <summary>
    /// frequent state changes, such as build progress
    /// </summary>
    private PolledQueue<WorldGridAttributeUpdateDTO> Updates { get; } = new();


    public event Action<ChunkRecord>? ChunkChangedEvent;

    public ChunkArray2D() 
    {
    }


    public bool Build(int x, int y, TileType buildTarget, float progress)
    {
        // only proceed if position is valid
        if (TryGetTile(x, y, out Tile? tile))
        {
            if (tile is null)
            {   //if no tile exists at position, make one with a build status.
                tile = new(TileType.Clear, -1, new BuildStatus(buildTarget, progress));
                return TrySetTile(x, y, tile);
            }
            else if (tile.Type == buildTarget) return false; // already constructed, no further action
            else if (tile.Building is BuildStatus status) 
            {   // If build status is not null, make progress on building status if the target is the same
                if (status.BuildTarget == buildTarget && status.BuildProgress < 1)
                {
                    status.BuildProgress += progress;
                    if (status.BuildProgress > 1) 
                        return TrySetTile(x, y, new(tile.Building.BuildTarget, 100)); //TODO temp HP value of 100
                    else Updates.Enqueue(WorldGridAttributeUpdateDTO.CreateBuildProgress(x, y, status.BuildProgress));
                    return true;
                }
                else return false;
            }
            else
            {   // if a tile exists, isn't the build target, and can be built, 
                tile = new(TileType.Clear, -1, new BuildStatus(buildTarget, progress));
                return TrySetTile(x, y, tile);
            }
        }

        return false;
    }

    //* Tile changes
    //*

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>. Tile may be null (default tile).
    /// </summary>
    public bool TryGetTile(int x, int y, out Tile? tile)
    {
        if (x >= 0 && y >= 0 && Chunks.TryGet(x / CommonDefines.ChunkSize, y / CommonDefines.ChunkSize, out Chunk? chunk)) 
        {
            tile = chunk[x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize];
            return true;
        }
        tile = default;
        return false;
    }

    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTile(int x, int y, Tile? tile)
    {
        if (Chunks.TryGet(x / CommonDefines.ChunkSize, y / CommonDefines.ChunkSize, out Chunk? chunk))
        {
            chunk[x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize] = tile;
            TileChanges.Enqueue(new SetCellCommand(new(tile is not null ? CommonToDTO.Convert(tile) : null, x, y)));
            return true;
        }
        return false;
    }

    //* Networking changes interface
    //*

    

    public IEnumerable<ChunkRecord> GetChunks()
    {
        for (byte x = 0; x < SizeX; x ++) for (byte y = 0; y < SizeY; y ++) yield return new ChunkRecord(this[x, y], x, y);
    }


    public void Execute(SetCellCommand setCell)
    {
        TrySetTile(setCell.CellRecord.X, setCell.CellRecord.Y, setCell.CellRecord.TileRecord is null ? null : CommonFromDTO.Convert(setCell.CellRecord.TileRecord));
    }
    public void SetChunk(ChunkRecord record)
    {
        if (Chunks.ContainsPosition(record.X, record.Y)) 
        {
            Chunks[record.X, record.Y] = record.Chunk;
            ChunkChangedEvent?.Invoke(record);
        }
    }

    /// <summary>
    /// Increases building progress for the cell at (<paramref name="x"/>, <paramref name="y"/>). Returns true if completed building or cannot build, and false if it is still progressing build. 
    /// </summary>
    bool IChunkArray2D.ProgressBuild(int x, int y, float progress)
    {
        if (TryGetTile(x, y, out Tile? tile) && tile is not null && tile.Building is BuildStatus status)
        {
            // throw new Exception();
            status.BuildProgress += progress;
            if (status.BuildProgress > 1)
            {
                return TrySetTile(x, y, new(tile.Building.BuildTarget, 100)); //TODO temp HP value of 100
            }
            else 
            {
                Updates.Enqueue(WorldGridAttributeUpdateDTO.CreateBuildProgress(x, y, status.BuildProgress));
                return false;
            }
        }
        return true;
    }

    void IChunkArray2D.StartBuild(int x, int y, TileType buildTarget, float initialProgress)
    {
        Tile tile = new(TileType.Clear, -1, new BuildStatus(buildTarget, initialProgress));
        TrySetTile(x, y, tile);
    }

}
