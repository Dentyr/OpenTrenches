using Godot;
using MessagePack;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

[MessagePackObject]
public record  class StructureDTO (
    [property: Key(0)] int X,
    [property: Key(1)] int Y,
    [property: Key(2)] StructureEnum Category,
    [property: Key(3)] float Health,
    [property: Key(4)] int Team
) : AbstractCreateDTO {}
