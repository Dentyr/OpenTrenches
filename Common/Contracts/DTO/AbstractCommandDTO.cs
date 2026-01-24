
using Godot;
using MessagePack;
namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
[Union(0, typeof(SetPlayerCommandDTO))]
[Union(10, typeof(ProjectileNotificationCommand))]
public abstract class AbstractCommandDTO
{
    
}

[MessagePackObject]
public class SetPlayerCommandDTO(ushort PlayerID) : AbstractCommandDTO
{
    [Key(0)]
    public ushort PlayerID { get; } = PlayerID;
}