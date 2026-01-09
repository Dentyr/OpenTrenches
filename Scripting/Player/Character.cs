
// namespace 
using System;
using Godot;
using OpenTrenches.Scripting.Multiplayer;
using OpenTrenches.Scripting.Datastream;
using MessagePack;
namespace OpenTrenches.Scripting.Player;

[MessagePackObject]
public class Character : IUpdateable
{
    [Key(0)]
    public Vector3 Position = new (0, 10, 0);

    [IgnoreMember]
    public Vector3 Movement = Vector3.Zero;


    public void Update(Update update)
    {
        switch ((UpdateType)update.Type)
        {
            case UpdateType.Position:
            Position = Serialization.Deserialize<Vector3>(update.Payload);
            break;
        }
    }
    public Update GetUpdate(UpdateType type)
    {
        byte[]? payload = null;
        switch (type)
        {
            case UpdateType.Position:
            payload = Serialization.Serialize<Vector3>(Position);
            break;
        }
        if (payload is null) throw new Exception();
        return new Update((byte)type, payload);
    }

    public enum UpdateType : byte
    {
        Position,
    }
}