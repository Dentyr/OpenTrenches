using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

public interface IServerChunkArray : IChunkArray2D<ServerChunk>
{
    IReadOnlyDictionary<int, ServerStructure> StructureDict { get; }

    TileConstruction? ProgressBuild(Vector2I buildCell, float progress);
    void StartBuild(Vector2I buildCell, TileType buildTarget, float initialProgress = 0);

    bool TryBuild(Vector2I buildCell, Team team, StructureEnum structure, out ServerStructure? result);
    
    public event Action<ServerStructure>? NewStructureEvent;


    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTile(int x, int y, [NotNullWhen(true)] out TileType? tile);
    public bool TryGetTile(Vector2I cell, [NotNullWhen(true)] out TileType? tile) => TryGetTile(cell.X, cell.Y, out tile);

    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell);
    public bool TryGetCell(Vector2I position, [NotNullWhen(true)] out CellRecord? cell) => TryGetCell(position.X, position.Y, out cell);
}
