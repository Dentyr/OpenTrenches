
using Godot;
using MessagePack;
namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
[Union(0, typeof(InputStatusDTO))]
public abstract class AbstractStreamDTO
{
    
}

[MessagePackObject]
public class InputStatusDTO : AbstractStreamDTO
{
    [Key(0)]
    public required UserKey[] Keys;
    /// <summary>
    /// Where the mouse is targeting in-world
    /// </summary>
    [Key(1)]
    public required Vector2 MousePos;
}

public enum UserKey : byte
{
    W,
    A,
    S,
    D,
    LMB,
    RMB,
    R,
    Build
}