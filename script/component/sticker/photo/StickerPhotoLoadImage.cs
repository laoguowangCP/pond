using System;
using System.Collections.Generic;
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

    // protected static readonly NodePath NP_TextureRect = "./EntityControl/PanelContainer/VBoxContainer/TextureRect";
    protected TextureRect TextureRect;

    public string ImageFile;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<TextureRect>(Name.NP_PhotoStickerTextureRect, out TextureRect);
    }

    public void LoadFromFile(string file)
    {
        ImageFile = file;
        GD.Print("Load image file: ", file);
        var imageTexture = ImageTexture.CreateFromImage(Image.LoadFromFile(file));
        TextureRect.Texture = imageTexture;

        if (Holder.TryGetComponent<ImageInfoLabel>(out var imageInfoLabel))
        {
            imageInfoLabel.UpdateInfo(file, imageTexture);
        }
    }

    public Vector2 GetImageTextureSize()
    {
        return TextureRect.Texture.GetSize();
    }
}