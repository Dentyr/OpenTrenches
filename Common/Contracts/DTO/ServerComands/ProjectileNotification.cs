using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;

[MessagePackObject]
public record class ProjectileNotificationCommand(
    [property: Key(0)] Vector2 Start,
    [property: Key(1)] Vector2 End,
    [property: Key(2)] ushort Character) : AbstractCommandDTO
{}