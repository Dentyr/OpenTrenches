using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Teams;
using OpenTrenches.Core.Scripting.World;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DeathScreen : Control
{
    private IClientState? _gameState;

    private Button _respawnButton = null!;
    public event Action? OnRespawnClicked;

    private float TimeLeft = 0f;

    /// <summary>
    /// The base the character will try to respawn at
    /// </summary>
    public ClientStructure? TargetBase { get; private set; }

    private Button _left = null!;
    private Button _right = null!;

    /// <summary>
    /// Invoked when the target base for player to respawn at has been changed
    /// </summary>
    public event Action<ClientStructure>? TargetBaseChangedEvetn;

    public override void _Ready()
    {
        _respawnButton = GetNode<Button>("Respawn");
        _respawnButton.Pressed += () => OnRespawnClicked?.Invoke();

        _left = GetNode<Button>("LeftBase");
        _left.Pressed += LastBase;
        _right = GetNode<Button>("RightBase");
        _right.Pressed += NextBase;
    }

    public void SetState(IClientState state)
    {
        _gameState = state;
    }

    public override void _Process(double delta)
    {
        TimeLeft -= (float)delta;
        
        if (
            !IsVisibleInTree() 
            || _gameState is null
            || _gameState.PlayerTeam is not ClientTeam team
        ) return;


        IEnumerable<ClientStructure> availableCamps = team.Camps.Where(camp => camp.Hp > 0);
        var availableCount = availableCamps.Count();

        // disabled/enable ability to switch between bases depending on number of existing bases
        if (availableCount == 0)
        {
            _respawnButton.Text = "Team Destroyed!";
            _respawnButton.Disabled = true;
            _left.Disabled = true;
            _right.Disabled = true;

            return;
        }

        if (availableCount == 1)
        {
            _left.Disabled = true;
            _right.Disabled = true;
        }
        else
        {
            _left.Disabled = false;
            _right.Disabled = false;
        }

        // if targetted respawn base is no longer available, swap to first available 
        if (!availableCamps.Contains(TargetBase))
        {
            SetTargetRespawn(availableCamps.First());
        }

        //* update respawnability based on timer

        if (TimeLeft > 0)
        {
            _respawnButton.Disabled = true;
            _respawnButton.Text = $"{Mathf.CeilToInt(TimeLeft):#} seconds left";
        }
        else
        {
            _respawnButton.Disabled = false;
            _respawnButton.Text = $"respawn";
        }
    }

    /// <summary>
    /// Show and prompt the user to respawn
    /// </summary>
    public void Prompt()
    {
        TimeLeft = CommonDefines.SecondsForCharacterRespawn;
        Visible = true;
    }

    private void NextBase()
    {
        SwitchBase(1);
    }
    private void LastBase()
    {
        SwitchBase(-1);
    }
    /// <summary>
    /// Gets all the available bases in a cyclic list, and gets the base <paramref name="offset"/> away from the current target base. If target base is null or not available, then get the first base
    /// </summary>
    private void SwitchBase(int offset)
    {
        if (_gameState is null || _gameState.PlayerTeam is null) return;
        List<ClientStructure> availableCamps = [.._gameState.PlayerTeam.Camps.Where(camp => camp.Hp > 0)];
        if (TargetBase is null || !availableCamps.Contains(TargetBase))
            SetTargetRespawn(availableCamps.First());
        else
        {
            int targetIdx = availableCamps.IndexOf(TargetBase);
            SetTargetRespawn(availableCamps[(targetIdx + offset + availableCamps.Count) % availableCamps.Count]);
        }
    }

    private void SetTargetRespawn(ClientStructure structure)
    {
        if (TargetBase != structure)
        {
            TargetBase = structure;
            TargetBaseChangedEvetn?.Invoke(structure);
        }
    }



}
