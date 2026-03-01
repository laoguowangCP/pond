using System;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.Save;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SaveStickerPhotoComponent : ComponentResource
{
    public override Type ComponentType => typeof(SaveStickerPhotoComponent);
    public override TickGroupEnum TickGroup => TickGroupEnum.Save;
    public override bool IsRegist => false;

    public override void Tick(TickContext ctx)
    {
        if (ctx.From.Is<SaveRoot>(out var save))
        {
            Holder.TryGetEntity<Node2D>(out var entity);
            Holder.TryGetNodeFromEntity<Control>("./EntityControl", out var control);
            SaveStickerPhoto photoSave = new(false);
            photoSave.GlobalPosition = entity.Position;
            photoSave.Size = control.Size;
            if (Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
            {
                photoSave.ImageFile = loadImage.ImageFile;
            }
            if (Holder.TryGetComponent<PhotoStickerPannerView>(out var pannerView))
            {
                photoSave.TextureRectCustomMinimumSize = pannerView.TextureRect.CustomMinimumSize;
                photoSave.HScrollValue = pannerView.ScrollContainer.ScrollHorizontal;
                photoSave.VScrollValue = pannerView.ScrollContainer.ScrollVertical;
            }
            save.TryAddListChildrenWithIdx(entity.GetIndex(), photoSave);
        }
    }

    public void Load(SaveStickerPhoto save)
    {
        Holder.TryGetEntity<Node2D>(out var entity);
        Holder.TryGetNodeFromEntity<Control>("./EntityControl", out var control);
        entity.GlobalPosition = save.GlobalPosition;
        control.Size = save.Size;
        if (Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
        {
            loadImage.LoadFromFile(save.ImageFile);
        }
        if (Holder.TryGetComponent<PhotoStickerPannerView>(out var pannerView))
        {
            pannerView.SetPannerViewInit(
                save.TextureRectCustomMinimumSize,
                save.HScrollValue,
                save.VScrollValue);
        }
    }
}
