using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Core.Scripting.Graphics;

public static class EquipmentTextureLibrary
{
    public static readonly IReadOnlyDictionary<EquipmentEnum, Texture2D> Textures = new Dictionary<EquipmentEnum, Texture2D>() {
        {
            EquipmentEnum.Rifle, 
            TextureLibrary2D.Equipment.RifleTexture
        },
        {
            EquipmentEnum.Shotgun, 
            TextureLibrary2D.Equipment.RifleTexture
        },
        {
            EquipmentEnum.MachineGun, 
            TextureLibrary2D.Equipment.RifleTexture
        },
    };
}