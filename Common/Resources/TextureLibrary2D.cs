using Godot;

namespace OpenTrenches.Common.Resources;
public static class TextureLibrary2D
{
    public static Texture2D StimulantThumbnail = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Stim.png");

    public static Texture2D NotFound { get; } = new Texture2D();

    public static Texture2D TransparentGray { get; } = ResourceLoader.Load<Texture2D>("Common/Resources/Img/DimGray.png");
    public static Texture2D Cyan { get; } = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Cyan.png");
    public static Texture2D Border { get; } = ResourceLoader.Load<Texture2D>("Common/Resources/Img/Border.png");


    public static class UI
    {
        public static Texture2D LogisticsThumbnail = ResourceLoader.Load<Texture2D>("Common/Resources/Img/UI/Logistics.png");
    }
}