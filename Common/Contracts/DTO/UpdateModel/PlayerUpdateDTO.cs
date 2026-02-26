using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class PlayerUpdateDTO(PlayerAttribute Attribute, byte[] Payload) 
: AbstractUpdateDTO<PlayerAttribute>(Attribute, Payload) {}

public enum PlayerAttribute : byte
{
    Logistics,
}
