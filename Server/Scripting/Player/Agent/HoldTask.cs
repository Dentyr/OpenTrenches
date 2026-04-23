using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Shoots at the closest enemy character.
/// </summary>
public class HoldTask : AbstractAgentTask
{
    private IWorldObject? _target;

    public HoldTask(IWorldObject? Target)
    {
        _target = Target;
    }

    public override AbstractAgentTask Reason(Character character, ICharacterAdapter adapter)
    {
        // if character is in middle of reloading or has run out of ammo, return
        if (!character.PrimarySlot.Reloaded) return this;
        if (character.PrimarySlot.AmmoStored == 0)
        {
            character.TryReload();
            return this;
        }

        _target = TaskServices.FindTarget(character, adapter);

        return this;
    }

    public override void Process(Character character, ICharacterAdapter adapter)
    {
        if (_target is null || 
            _target.Hp <= 0 ||
            _target.Position.DistanceSquaredTo(character.Position) > 400
         ) 
        {
            _target = null;
            character.TryClear(CharacterState.Shooting);
            return;
        }

        character.Direction = _target.Position;
        character.TrySet(CharacterState.Shooting);

        
    }
}
