using System;

namespace OpenTrenches.Common.World;

/// <summary>
/// Describes a structure
/// </summary>
public readonly record struct StructureRecord (
    int X,
    int Y,
    int ID
) {}