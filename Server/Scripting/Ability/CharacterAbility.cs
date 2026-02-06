using System;
using OpenTrenches.Common.Ability;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Ability;


public class CharacterAbility(AbilityEffectRecord Record) : ActivatedAbility(Record.Info)
{
    private Func<Character, bool> CanUse { get; } = Record.CanUse;

    public bool CanDo(Character character) => TimeLeft <= 0 && CanUse(character);
    public bool TryDo(Character character) {
        if (CanDo(character))
        {
            ActivateTimer();
            return true;
        }  
        return false;
    } 

}
