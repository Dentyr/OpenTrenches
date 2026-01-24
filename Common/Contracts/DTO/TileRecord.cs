using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;


[MessagePackObject]
public record class TileRecord(
    [property: Key(0)] TileType Tile, 
    [property: Key(1)] float Health,
    [property: Key(3)] BuildingRecord? Building
);

/// <summary>
/// A <paramref name="BuildTarget"/> with <paramref name="Health"/> under construction with <paramref name="BuildProgress"/> 
/// </summary>
[MessagePackObject]
public record class BuildingRecord(
    [property: Key(0)] TileType BuildTarget, 
    [property: Key(1)] float Health, 
    [property: Key(2)] float BuildProgress
);