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
[Union(101,     typeof(ReloadNotificationCommand))]
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
[Union(204,     typeof(PurchaseCommandRequest))]
[Union(205,     typeof(ExitTrenchCommandRequest))]


[Union(9900,     typeof(GameEndNotificationCommand))]
public abstract record class AbstractCommandDTO {}
