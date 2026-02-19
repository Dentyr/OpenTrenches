using Godot;
using OpenTrenches.Common.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

[GlobalClass]
[Tool]
public partial class IconDisplay : Control
{
    [Export]
    public string Text
    {
        get => _label.Text;
        set => _label.Text = value;
    }
    [Export]
    public Texture2D Texture
    {
        get => _texture.Texture;
        set => _texture.Texture = value;
    }

    private Label _label;
    private TextureRect _texture;

    public IconDisplay()
    {
        _texture = new()
        {
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize
        };
        _texture.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        _label = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _label.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        Text = "";


        AddChild(_texture);
        AddChild(_label);
    }
}
