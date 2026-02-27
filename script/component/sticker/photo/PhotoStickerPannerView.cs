using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class PhotoStickerPannerView : ComponentResource
{
    public override Type ComponentType => typeof(PhotoStickerPannerView);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected TextureRect TextureRect;
    protected ScrollContainer ScrollContainer;

    protected float ZoomLevel = 1f;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<TextureRect>(Name.NP_PhotoStickerTextureRect, out TextureRect);
        TextureRect.GuiInput += OnGuiInput;
        ScrollContainer = TextureRect.GetParentOrNull<ScrollContainer>();
        return true;
    }

    public override bool OnHolderTryRemove()
    {
        TextureRect.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (Input.IsActionJustPressed(Name.SN_CtrlScrollUp))
            {
                SetZoomLevel(1.13f);
                TextureRect.AcceptEvent();
            }
            else if (Input.IsActionJustPressed(Name.SN_CtrlScrollDown))
            {
                SetZoomLevel(1f/1.13f);
                TextureRect.AcceptEvent();
            }
        }
    }

    protected void SetZoomLevel(float level)
    {
        /*
        TODO: 添加 zoom in 上限
        */
        // GD.Print($"TextureRect size: {TextureRect.Size}, ScrollContainer size: {ScrollContainer.Size}");
        Vector2 rectSize = TextureRect.Size;
        // TODO: 参考图像自身的长宽比进行放大/缩小
        Vector2 textureSize = TextureRect.Texture.GetSize();
        float textureXYRatio = textureSize.X / textureSize.Y;
        Vector2 nextMinimumSize = new();
        if (level >= 1f) // Zoom in
        {
            if (rectSize.X >= textureXYRatio * rectSize.Y)
            {
                // X should not grow that much
                nextMinimumSize.Y = rectSize.Y * level;
                nextMinimumSize.X = MathF.Max(nextMinimumSize.Y * textureXYRatio, rectSize.X);
            }
            else
            {
                // Y should not grow that much
                nextMinimumSize.X = rectSize.X * level;
                nextMinimumSize.Y = MathF.Max(nextMinimumSize.X / textureXYRatio, rectSize.Y);
            }
        }
        else // Zoom out
        {
            nextMinimumSize = rectSize * level;
            /*
            if (rectSize.X >= textureXYRatio * rectSize.Y)
            {
                // Y should not reduce that much
                nextMinimumSize.X = rectSize.X * level;
                nextMinimumSize.Y = MathF.Min(nextMinimumSize.X / textureXYRatio, rectSize.Y);
            }
            else
            {
                // X should not reduce that much
                nextMinimumSize.Y = rectSize.Y * level;
                nextMinimumSize.X = MathF.Min(nextMinimumSize.Y * textureXYRatio, rectSize.X);
            }
            */
        }
        if (nextMinimumSize.X <= ScrollContainer.Size.X
            && (nextMinimumSize.Y <= ScrollContainer.Size.Y))
        {
            nextMinimumSize = Vector2.Zero;
        }
        TextureRect.CustomMinimumSize = nextMinimumSize;
    }
}
