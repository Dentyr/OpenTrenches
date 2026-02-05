using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record class SetCellCommand(
    [property: Key(0)] CellRecord CellRecord
) : AbstractCommandDTO {}
