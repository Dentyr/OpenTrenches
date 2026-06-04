using Godot;

namespace OpenTrenches.Common.Resources;
public static class TextureLibrary2D
{

    public static Texture2D NotFound { get; } = new Texture2D();

    public static Texture2D TransparentGray { get; } = ResourceLoader.Load<Texture2D>("Common/Resources/Img/DimGray.png");
    public static Texture2D Cyan { get; } = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Cyan.png");
    public static Texture2D Border { get; } = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Border.png");

    public static class Ability
    {
        
        public static Texture2D Airstrike = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Airstrike.png");
        public static Texture2D Stimulant = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Stim.png");
    }
    public static class UI
    {
        public static Texture2D LogisticsThumbnail = ResourceLoader.Load<Texture2D>("Common/Resources/Img/UI/Logistics.png");
    }

    public static class Equipment
    {
        public static Texture2D RifleTexture = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Equipment/Rifle.png");
        public static Texture2D MachineGunTexture = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Equipment/MachineGun.png");
        public static Texture2D ShotGunTexture = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Equipment/Shotgun.png");
    }

    public static class Character
    {
        public static Texture2D DefaultCharacter = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Default.png");

        public static Texture2D Rifle = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Weapon/Rifle.png");
        public static Texture2D Shotgun = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Weapon/Shotgun.png");
        public static Texture2D Machingun = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Weapon/Machinegun.png");

        public static Texture2D RifleFired = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Weapon/RifleFired.png");
        public static Texture2D ShotgunFired = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Weapon/ShotgunFired.png");
        public static Texture2D MachingunFired = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Character/Weapon/MachinegunFired.png");
    }

    public static class Structure
    {
        public static Texture2D Camp = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Structure/Camp.png");
    }
}