
using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record class BuildCommandRequest(
    [property: Key(0)] int X,
    [property: Key(1)] int Y,
    [property: Key(2)] TileType Tile
) : AbstractCommandDTO {}