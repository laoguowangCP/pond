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

    protected OnStickerRemove OnStickerRemove;

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<OnStickerRemove>(out OnStickerRemove);
        OnStickerRemove.StickerRemove += DeleteImageFile;
    }

    public override bool OnHolderTryRemove()
    {
        OnStickerRemove.StickerRemove -= DeleteImageFile;
        // Simply return true.
        // Or remove other component you want.
        return base.OnHolderTryRemove();
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