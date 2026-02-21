
using Godot;
using MessagePack;
namespace OpenTrenches.Common.Contracts.DTO.PlayerCommands;

[MessagePackObject]
[Union(0, typeof(InputStatusDTO))]
public abstract record class AbstractStreamDTO {}

/// <summary>
/// Represents the input state of the user
/// </summary>
/// <param name="Keys">Keys pressed by the user</param>
/// <param name="MousePos">Position of mouse within the world</param>
[MessagePackObject]
public record class InputStatusDTO(
    [property: Key(0)] UserKey[] Keys,
    [property: Key(1)] Vector2 MousePos
) : AbstractStreamDTO
{}

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