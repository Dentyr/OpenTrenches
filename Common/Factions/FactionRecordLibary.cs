using System;

namespace OpenTrenches.Common.Factions;

public static class FactionRecordLibary
{
    

    public static readonly FactionRecord StandardFaction = new(FactionEnum.StandardDebug)
    {
        Name="Debug Name",
        Description="Debug Desc",
    };
}


public enum FactionEnum : int
{
    StandardDebug
}