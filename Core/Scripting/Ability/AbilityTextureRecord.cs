using Godot;
using OpenTrenches.Common.Ability;

namespace OpenTrenches.Core.Scripting.Ability;

public record class AbilityTextureRecord(
    AbilityRecord Info,
    Texture2D Thumbnail
) {}