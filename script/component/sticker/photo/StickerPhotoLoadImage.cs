using System;
using System.Collections.Generic;
using System.IO;
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
    protected ImageTexture ImageTextureRes;

    public string ImageFile;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<TextureRect>(Name.NP_PhotoStickerTextureRect, out TextureRect);
    }

    public void LoadFromFile(string file)
    {
        ImageFile = file;
        if (!File.Exists(ImageFile))
        {
            return;
        }
        GD.Print("Load image file: ", file);
        using var image = Image.LoadFromFile(file);
        ImageTextureRes = ImageTexture.CreateFromImage(image);
        TextureRect.Texture = ImageTextureRes;

        if (Holder.TryGetComponent<ImageInfoLabel>(out var imageInfoLabel))
        {
            imageInfoLabel.UpdateInfo(file, ImageTextureRes);
        }
    }

    public override bool OnHolderTryRemove()
    {
        TextureRect.Texture = null;
        ImageTextureRes.Dispose();
        return base.OnHolderTryRemove();
    }

    public Vector2 GetImageTextureSize()
    {
        return TextureRect.Texture.GetSize();
    }
}