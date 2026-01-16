
// namespace 
using System;
using Godot;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Core.Scripting.Player;

public class Character : IIdObject
{
    public Vector3 Position { get; set; } = new (0, 10, 0);

    public float Health { get; set; } = 10f;

    public ushort ID { get; }

    public Character(ushort ID, Vector3 Position, float Health)
    {
        this.ID = ID;
        this.Position = Position;
        this.Health = Health;
    }

    //* Updates

    public void Update(Update update)
    {
        switch ((CharacterAttribute)update.Type)
        {
            case CharacterAttribute.Position:
                Position = CharacterUpdateHelper.FromPositionUpdate(update.Payload);
                break;
            case CharacterAttribute.Health:
                Health = CharacterUpdateHelper.FromHealthUpdate(update.Payload);
                break;
        }
    }
}