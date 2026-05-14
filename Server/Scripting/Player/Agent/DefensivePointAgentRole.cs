namespace OpenTrenches.Server.Scripting.Player.Agent;

public enum DefensivePointAgentRole
{
    /// <summary>
    /// Sappers expanded trenches. When the trench system is complete, they become holders
    /// </summary>
    Sapper,
    /// <summary>
    /// Holders defend a position
    /// </summary>
    Holder,
    /// <summary>
    /// Testers occasionally jump forward to test for enemy activity, inviting more to come if same.
    /// </summary>
    Tester,
}