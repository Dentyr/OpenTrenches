using Godot;

namespace OpenTrenches.Common.Contracts.Defines;
public static class CommonDefines
{
    /// <summary>
    /// Areas are higher level divisions containing cells used for AI behavior
    /// </summary>
    public const byte AreaSize = 16;

    /// <summary>
    /// Number of areas the map has horizontally
    /// </summary>
    public const byte WorldLengthArea = 10;
    /// <summary>
    /// Number of areas the map has vertically
    /// </summary>
    public const byte WorldHeightArea = 8;

    /// <summary>
    /// Number of cells the map has horizontally
    /// </summary>
    public const byte WorldLength = AreaSize * WorldLengthArea;
    /// <summary>
    /// Number of cells the map has vertically
    /// </summary>
    public const byte WorldHeight = AreaSize * WorldHeightArea;

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