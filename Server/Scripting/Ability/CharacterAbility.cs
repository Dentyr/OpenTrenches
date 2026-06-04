using System;
using OpenTrenches.Common.Ability;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Ability;


public class CharacterAbility(AbilityRecord Record) : ActivatedAbility(Record)
{
    private Func<Character, bool> CanUse { get; } = AbilityEffectRecords.Effects[Record].CanUse;
    public Action<Character, ICharacterAdapter> FinishEffect => AbilityEffectRecords.Effects[Record].FinishEffect;

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
