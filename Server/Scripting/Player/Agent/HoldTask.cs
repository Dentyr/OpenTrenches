using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Shoots at the closest enemy character.
/// </summary>
public class HoldTask : AbstractAgentTask
{
    private IWorldObject? _currentTarget;

    public HoldTask(IWorldObject? Target = null)
    {
        _currentTarget = Target;
    }

    public override AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService)
    {
        _currentTarget = TaskServices.FindTarget(character, queryService);
        TaskServices.ReasonAttack(character);

        return this;
    }

    public override bool Process(Character character, IWorld2DQueryService queryService)
    {
        if (TaskServices.EnemyValid(character, _currentTarget, 20)) 
        {
            character.Direction = _currentTarget.Position;
            character.TrySet(CharacterState.Shooting);
        }
        else
        {
            _currentTarget = null;
            character.TryClear(CharacterState.Shooting);
        }

        return false;
    }
}
