
// namespace 
using System;
using Godot;
using OpenTrenches.Scripting.Multiplayer;

public class Character : IUpdateable
{
    public Vector3 Position = new (0, 10, 0);

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

    public enum UpdateType : byte
    {
        Position,
    }
}