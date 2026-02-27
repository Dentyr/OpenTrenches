using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Core.Scripting.Graphics;

public static class EquipmentTextureLibrary
{
    public static readonly IReadOnlyDictionary<FirearmEnum, Texture2D> Textures = new Dictionary<FirearmEnum, Texture2D>() {
        {
            FirearmEnum.Rifle, 
            TextureLibrary2D.Equipment.RifleTexture
        },
        {
            FirearmEnum.Shotgun, 
            TextureLibrary2D.Equipment.ShotGunTexture
        },
        {
            FirearmEnum.MachineGun, 
            TextureLibrary2D.Equipment.MachineGunTexture
        },
    };
}