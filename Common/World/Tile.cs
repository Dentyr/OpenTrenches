namespace OpenTrenches.Common.World;
public class Tile(TileType Type, TileConstruction? BuildStatus = null)
{
    public TileType Type { get; } = Type;
    public TileConstruction? BuildStatus { get; } = BuildStatus;

    public override string ToString() => $"{Type}, {BuildStatus}";
}

/// <summary>
/// A tile currently being updated into a different tile type
/// </summary>
public class TileConstruction(TileType Target, float InitialProgress, float InitialDecay = 0) : ITileConstruction
{
    public TileType Target { get; } = Target;
    public float Progress { get; set; } = InitialProgress;
    
    /// <summary>
    /// A marker for how long it's been since this has been updated. Intended to be used for disposal of abandoned construction statuses
    /// </summary>
    public float Decay { get; set; } = InitialDecay;
}

/// <summary>
/// Record of a tile being changed
/// </summary>
public class TileConstructionRecord(int X, int Y, TileConstruction Status)
{
    public int X = X;
    public int Y = Y;
    public TileConstruction Status = Status;
}


public interface ITileConstruction
{
    public TileType Target { get; }
    public float Progress { get; }
}