using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class SetCellCommand(
    [property: Key(0)] int X,
    [property: Key(1)] int Y,
    [property: Key(2)] TileType Tile
) : AbstractCommandDTO {}
