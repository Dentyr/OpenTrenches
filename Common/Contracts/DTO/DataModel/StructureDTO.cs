using Godot;
using MessagePack;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

[MessagePackObject]
public record  class StructureDTO (
    [property: Key(0)] int Id,
    [property: Key(1)] int Team,
    [property: Key(2)] int X,
    [property: Key(3)] int Y,
    [property: Key(4)] StructureEnum Category,
    [property: Key(5)] float Health
) : AbstractCreateDTO {}
