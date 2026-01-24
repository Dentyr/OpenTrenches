using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public class ProjectileNotificationCommand(Vector3 Start, Vector3 End) : AbstractCommandDTO
{
    [Key(0)]
    public Vector3 Start { get; } = Start;
    [Key(1)]
    public Vector3 End { get; } = End;
}