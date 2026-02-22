using System;
using System.IO;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class PhotoStickerRemoveDeleteFile : ComponentResource
{
    public override Type ComponentType => typeof(PhotoStickerRemoveDeleteFile);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<OnStickerRemove>(out var onStickerRemove);
        onStickerRemove.StickerRemove += DeleteImageFile;
    }

    private void DeleteImageFile()
    {
        Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage);
        if (File.Exists(loadImage.ImageFile))
        {
            File.Delete(loadImage.ImageFile);
        }
    }
}