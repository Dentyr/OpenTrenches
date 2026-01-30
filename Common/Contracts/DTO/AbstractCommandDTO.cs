
using Godot;
using MessagePack;
namespace OpenTrenches.Common.Contracts.DTO;

/// <summary>
/// DTO for commands the target reliably receive
/// </summary>
[MessagePackObject]
[Union(0, typeof(SetPlayerCommandDTO))]
[Union(10, typeof(ProjectileNotificationCommand))]
public abstract record class AbstractCommandDTO {}

[MessagePackObject]
public record class SetPlayerCommandDTO(
    [property: Key(0)] ushort PlayerID
) : AbstractCommandDTO {}

[MessagePackObject]
public record class BuiltCommand(
    [property: Key(1)] int X, 
    [property: Key(2)] int Y
) : AbstractCommandDTO {}