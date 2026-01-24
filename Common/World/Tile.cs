using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.World;
public class Tile(TileType Type, float Health)
{
    public TileType Type { get; } = Type;
    public float Health { get; } = Health;
}