using System;
using Godot;
using OpenTrenches.Scripting.Player;
using OpenTrenches.Scripting.Multiplayer;

namespace OpenTrenches.Scripting.Multiplayer;

public class PlayerNetworkHandler : AbstractNetworkHandler
{
    private ServerState GameState { get; }

    public ushort CharacterId;
    public Character Character => GameState.Characters[CharacterId];

    public PlayerNetworkHandler(INetworkConnectionAdapter Adapter, ServerState GameState) : base(Adapter)
    {
        this.GameState = GameState;
        CharacterId = GameState.CreateCharacter(new Character());
    }

    #region create
    protected override void _DeserializeCreate(CreateDatagram datagram)
    {
    }
    #endregion
    #region stream
    protected override void _DeserializeStream(StreamDatagram datagram)
    {
        switch (datagram.StreamCategory)
        {
            case StreamCategory.Input:
                if (Serialization.TryDeserialize<InputStatus>(datagram.Item, out var input)) InterpretInput(input);
                break;
        }
    }
    private void InterpretInput(InputStatus input)
    {
        Vector3 movement = Vector3.Zero;
        foreach (UserKey key in input.Keys)
        {
            switch(key)
            {
                case UserKey.W:
                movement.Z -= 1;
                break;
                case UserKey.A:
                movement.X -= 1;
                break;
                case UserKey.S:
                movement.Z += 1;
                break;
                case UserKey.D:
                movement.X += 1;
                break;
            }
        }
        movement *= 250f;
        Character.Movement = movement;
    }
    #endregion
    #region update
    protected override void _DeserializeUpdate(UpdateDatagram datagram)
    {
        throw new NotImplementedException();
    }
    #endregion
    
    protected override void _DeserializeMessage(MessageDatagram message)
    {
        throw new NotImplementedException();
    }
}