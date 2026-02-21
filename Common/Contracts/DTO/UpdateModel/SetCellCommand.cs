using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class SetCellCommand(
    [property: Key(0)] CellRecord CellRecord
) : AbstractCommandDTO {}
