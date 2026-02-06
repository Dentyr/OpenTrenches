using System;
using Godot;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using System.Linq;
using OpenTrenches.Common.Contracts.Defines;
using System.Runtime.InteropServices;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;

namespace OpenTrenches.Server.Scripting.Adapter;

public class PlayerNetworkHandler : AbstractNetworkHandler
{
    private ServerState GameState { get; }

    public Character Character { get; private set; }

    public PlayerNetworkHandler(INetworkConnectionAdapter Adapter, ServerState GameState) : base(Adapter)
    {
        this.GameState = GameState;
        Character = GameState.CreateCharacter();
    }



    #region create
    protected override void _DeserializeCreate(CreateDatagram datagram)
    {
    }
    #endregion
    #region stream
    protected override void _DeserializeStream(StreamDatagram datagram)
    {
        if (datagram.DTO is InputStatusDTO input) InterpretInput(input);
    }
    private void InterpretInput(InputStatusDTO input)
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
        if (input.Keys.Contains(UserKey.LMB)) Character.TrySwitch(CharacterState.Shooting);
        else if (Character.State == CharacterState.Shooting) Character.TrySwitch(CharacterState.Idle);

        movement *= 5f;
        Character.MovementVelocity = movement;

        Character.Direction = new(input.MousePos.X, Character.Position.Y, input.MousePos.Y);
    }
    #endregion
    #region update
    protected override void _DeserializeUpdate(UpdateDatagram datagram)
    {
        throw new NotImplementedException();
    }

    protected override void _DeserializeMessage(CommandDatagram message)
    {
        AbstractCommandDTO command = message.DTO;
        if (command is BuildCommandRequest buildCommand)
        {
            Character.SetBuildTarget(buildCommand.X, buildCommand.Y, buildCommand.Tile);   
        }
        else if (command is UseAbilityCommandRequest commandRequest)
        {
            Character.TryActivate(commandRequest.Idx);
        }
    }
    #endregion

}