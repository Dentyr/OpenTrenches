using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Combat;

public static class CombatCalculationService
{
    /// <summary>
    /// Calculates whether or not <paramref name="origin"/> will hit <paramref name="target"/>.
    /// </summary>
    public static bool TryHit(WorldLayer fireOrigin, Character target)
    {
        /// <summary>
        /// Same layer fire has a 100% chance of hitting
        /// </summary>
        if (fireOrigin == target.Layer) return true;

        /// <summary>
        /// Characters aiming out of a trench have a chance of being hit by ground fire
        /// </summary>
        /// <param name="="></param>
        /// <returns></returns>
        if (fireOrigin == WorldLayer.Ground && 
            target.Layer == WorldLayer.Trench && 
            target.State.HasFlag(Common.Contracts.Defines.CharacterState.Aiming))
        {
            return GD.Randf() > 0.5;
        }
        return false;
    }
}