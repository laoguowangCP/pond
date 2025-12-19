using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class StickerPhotoLoadImage : ComponentResource
{
    public override Type ComponentType => typeof(StickerPhotoLoadImage);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_TextureRect = new("EntityControl/PanelContainer/TextureRect");
    protected TextureRect TextureRect;

    public override void OnEntityReady()
    {
        Holder.TryGetNode<TextureRect>(NP_TextureRect, out TextureRect);
    }

    public void LoadFromFile(string file)
    {
        GD.Print("Load image file: ", file);
        var imageTexture = ImageTexture.CreateFromImage(Image.LoadFromFile(file));
        TextureRect.Texture = imageTexture;
    }
}