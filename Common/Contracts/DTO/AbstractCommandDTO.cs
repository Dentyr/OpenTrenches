
using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
namespace OpenTrenches.Common.Contracts.DTO;

/// <summary>
/// DTO for commands the target reliably receive
/// </summary>
[MessagePackObject]
[Union(100,     typeof(ProjectileNotificationCommand))]
[Union(110,     typeof(AbstractCommandDTO))]
[Union(111,     typeof(AbilityNotificationCommand))]
[Union(199,     typeof(InitializedNotificationCommand))]

[Union(140,     typeof(DeathNotificationCommand))]
[Union(141,     typeof(RespawnNotificationCommand))]

[Union(500,     typeof(SetPlayerCommandDTO))]
[Union(501,     typeof(SetCellCommand))]



[Union(200,     typeof(RespawnCommandRequest))]
[Union(201,     typeof(BuildCommandRequest))]
[Union(202,     typeof(UseAbilityCommandRequest))]
[Union(203,     typeof(ReloadCommandRequest))]
public abstract record class AbstractCommandDTO {}

[MessagePackObject]
public record class SetPlayerCommandDTO(
    [property: Key(0)] ushort PlayerID
) : AbstractCommandDTO {}

[MessagePackObject]
public record class BuildCommandRequest(
    [property: Key(0)] int X,
    [property: Key(1)] int Y,
    [property: Key(2)] TileType Tile
) : AbstractCommandDTO {}