using OpenTrenches.Common.Contracts.DTO.UpdateModel;

namespace OpenTrenches.Common.World;
public class Tile(TileType Type, float Health, BuildStatus? Building = null)
{
    public TileType Type { get; } = Type;
    public float Health { get; } = Health;
    public BuildStatus? Building { get; } = Building;

    public override string ToString() => $"{Type}, {Health}, {Building}";
}

public class BuildStatus(TileType BuildTarget, float BuildProgress)
{
    public TileType BuildTarget {get; } = BuildTarget;
    public float BuildProgress {get; set; } = BuildProgress;
}