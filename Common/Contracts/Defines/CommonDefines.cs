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
    /// <summary>
    /// Pixels a character spans
    /// </summary>
    public const float CharacterSize = 16f;
    public const float CharacterRadius = CharacterSize / 2;

    public const float MaxHp = 25f;
}