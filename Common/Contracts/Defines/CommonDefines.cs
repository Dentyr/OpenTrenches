using Godot;

namespace OpenTrenches.Common.Contracts.Defines;
public static class CommonDefines
{
    /// <summary>
    /// Cells a chunk spans
    /// </summary>
    public const byte ChunkSize = 32; 
    public const byte WorldSize = 4;

    /// <summary>
    /// Pixels a cell spans
    /// </summary>
    public const float CellSize = 32f;

    public const float MaxHealth = 25f;
}