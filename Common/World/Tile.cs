namespace OpenTrenches.Common.World;
public class Tile(TileType Type, BuildStatus? Building = null)
{
    public TileType Type { get; } = Type;
    public BuildStatus? Building { get; } = Building;

    public override string ToString() => $"{Type}, {Building}";
}

public class BuildStatus(TileType BuildTarget, float BuildProgress)
{
    public TileType BuildTarget {get; } = BuildTarget;
    public float BuildProgress {get; set; } = BuildProgress;
}
