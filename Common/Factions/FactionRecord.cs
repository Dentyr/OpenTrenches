namespace OpenTrenches.Common.Factions;

public sealed record class FactionRecord(FactionEnum ID)
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}