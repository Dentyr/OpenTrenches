namespace OpenTrenches.Server.Scripting.Player.Agent;

public record class AgentAssignmentRecord(
    CharacterAgent Agent,
    AgentRole Role
);