namespace OpenTrenches.Common.World;

public static class StructureTypes
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StructureType BaseCamp = new(StructureEnum.Camp, new(-1, -1), new(1, 1))
    {
        Constructable = false,
        HitPoints = 1000,
    };
}