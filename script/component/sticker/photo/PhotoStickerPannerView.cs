using Godot;
using System;
using LGWCP.NiceGD;
using System.Threading.Tasks;
using GodotTask;

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
    protected OnStickerSizeChanged OnStickerSizeChanged;

    protected float ZoomLevel = 1f;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<TextureRect>(Name.NP_PhotoStickerTextureRect, out TextureRect);
        TextureRect.GuiInput += OnGuiInput;
        TextureRect.MouseEntered += OnMouseEntered;
        TextureRect.MouseExited += OnMouseExited;
        ScrollContainer = TextureRect.GetParentOrNull<ScrollContainer>();
        return true;
    }

    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
        Holder.TryGetComponent<OnStickerSizeChanged>(out OnStickerSizeChanged);
        OnStickerSizeChanged.StickerSizeChanged += CheckShouldResetMinimumSize;
    }

    public override bool OnHolderTryRemove()
    {
        TextureRect.GuiInput -= OnGuiInput;
        TextureRect.MouseEntered -= OnMouseEntered;
        TextureRect.MouseExited -= OnMouseExited;
        OnStickerSizeChanged.StickerSizeChanged -= CheckShouldResetMinimumSize;
        return base.OnHolderTryRemove();
    }

    public override void Tick(TickContext ctx)
    {
        if (Input.IsActionJustReleased(Name.SN_CtrlScrollUp, true))
        {
            SetZoomLevel(1.1f);
            TextureRect.AcceptEvent();
        }
        else if (Input.IsActionJustReleased(Name.SN_CtrlScrollDown, true))
        {
            SetZoomLevel(1f/1.1f);
            TextureRect.AcceptEvent();
        }
    }


    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (Input.IsActionJustPressed(Name.SN_Ctrl))
            {
                GD.Print("Ctrl is pressed down.");
            }
            if (Input.IsActionJustReleased(Name.SN_CtrlScrollUp, true))
            {
                SetZoomLevel(1.1f);
                TextureRect.AcceptEvent();
            }
            else if (Input.IsActionJustReleased(Name.SN_CtrlScrollDown, true))
            {
                SetZoomLevel(1f/1.1f);
                TextureRect.AcceptEvent();
            }
        }
    }

    private void OnMouseEntered()
    {
        // throw new NotImplementedException();
        GD.Print("OnMouseEntered");
    }

    private void OnMouseExited()
    {
        // throw new NotImplementedException();
        GD.Print("OnMouseExited");
    }

    protected void SetZoomLevel(float level)
    {
        /*
        TODO: 添加 zoom in 上限
        */
        // GD.Print($"TextureRect size: {TextureRect.Size}, ScrollContainer size: {ScrollContainer.Size}");
        TextureRect.UpdateMinimumSize();
        ScrollContainer.GetMinimumSize();
        ScrollContainer.DrawFocusBorder = true;
        ScrollContainer.DrawFocusBorder = false;
        Vector2 rectSize = TextureRect.Size;
        // TODO: 参考图像自身的长宽比进行放大/缩小
        Vector2 textureSize = TextureRect.Texture.GetSize();
        float textureXYRatio = textureSize.X / textureSize.Y;
        Vector2 customMinimumSize = TextureRect.CustomMinimumSize.Max(rectSize);
        Vector2 nextMinimumSize;
        if (level >= 1f) // Zoom in
        {
            if (customMinimumSize.X >= textureXYRatio * customMinimumSize.Y)
            {
                // X should not grow that much
                /*
                nextMinimumSize.Y = rectSize.Y * level;
                nextMinimumSize.X = nextMinimumSize.Y * textureXYRatio;
                */
                customMinimumSize.X = customMinimumSize.Y * textureXYRatio;
                nextMinimumSize = customMinimumSize * level;
            }
            else
            {
                // Y should not grow that much
                /*
                nextMinimumSize.X = rectSize.X * level;
                nextMinimumSize.Y = nextMinimumSize.X / textureXYRatio;
                */
                customMinimumSize.Y = customMinimumSize.X / textureXYRatio;
                nextMinimumSize = customMinimumSize * level;
            }
        }
        else // Zoom out
        {
            nextMinimumSize = customMinimumSize * level;
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
        /*
        if (nextMinimumSize.X <= ScrollContainer.Size.X
            && (nextMinimumSize.Y <= ScrollContainer.Size.Y))
        {
            nextMinimumSize = Vector2.Zero;
        }
        */
        // Caluculate position
        int scrollH = ScrollContainer.ScrollHorizontal;
        int scrollV = ScrollContainer.ScrollVertical;
        Vector2 mousePosPanner = ScrollContainer.GetLocalMousePosition();
        Vector2 mousePosTexture = TextureRect.GetLocalMousePosition();
        /*
        GD.Print(scrollH);
        GD.Print(scrollV);
        GD.Print(mousePosPanner);
        GD.Print(mousePosTexture);
        */
        Vector2 ratioPos = mousePosTexture / customMinimumSize;
        // ratioPos = ratioPos.Clamp(Vector2.Zero, textureSize);
        Vector2 nextMousePosTexture = ratioPos * nextMinimumSize;
        Vector2 nextScroll = nextMousePosTexture - mousePosPanner;
        
        GD.Print($"Set custom minimum size: {nextMinimumSize}");
        TextureRect.CustomMinimumSize = nextMinimumSize;
        TextureRect.UpdateMinimumSize();
        // TextureRect.GetMinimumSize();
        ScrollContainer.GetMinimumSize();
        ScrollContainer.DrawFocusBorder = true;
        ScrollContainer.DrawFocusBorder = false;
        // ScrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
        // ScrollContainer.HorizontalScrollMode = ScrollContainer.ScrollMode.Reserve;
        // ScrollContainer.VerticalScrollMode = ScrollContainer.ScrollMode.Disabled;
        // ScrollContainer.VerticalScrollMode = ScrollContainer.ScrollMode.Reserve;
        // ScrollContainer.GetMinimumSize();
        // ScrollContainer.ScrollHorizontal = (int)nextMinimumSize.X;
        // ScrollContainer.ScrollVertical = (int)nextMinimumSize.Y;
        // await GDTask.NextFrame();
        
        // await ToSignal(ScrollContainer.GetTree(), SceneTree.SignalName.ProcessFrame);
        // ScrollContainer.QueueRedraw();
        // Callable.From(ScrollFollowMousePos).CallDeferred();
        ScrollFollowMousePos();

        void ScrollFollowMousePos()
        {
            HScrollBar hScrollBar = ScrollContainer.GetHScrollBar();
            VScrollBar vScrollBar = ScrollContainer.GetVScrollBar();
            /*
            if (Mathf.IsZeroApprox(hScrollBar.MaxValue))
                hScrollBar.MaxValue = nextScroll.X;
            if (Mathf.IsZeroApprox(vScrollBar.MaxValue))
                vScrollBar.MaxValue = nextScroll.Y;
            */
            GD.Print($"Scroll limit: ({hScrollBar.MaxValue}, {vScrollBar.MaxValue})");
            ScrollContainer.ScrollHorizontal = Mathf.RoundToInt(Mathf.Clamp(nextScroll.X, hScrollBar.MinValue, hScrollBar.MaxValue));
            ScrollContainer.ScrollVertical = Mathf.RoundToInt(Mathf.Clamp(nextScroll.Y, vScrollBar.MinValue, vScrollBar.MaxValue));
            GD.Print($"Scroll value change: {scrollH}, {scrollV} -> {ScrollContainer.ScrollHorizontal}, {ScrollContainer.ScrollVertical}");
            ScrollContainer.DrawFocusBorder = false;
            CheckShouldResetMinimumSize();
        }
    }

    public void CheckShouldResetMinimumSize()
    {
        var minimumSize = TextureRect.CustomMinimumSize;
        if (minimumSize.X <= ScrollContainer.Size.X
            && (minimumSize.Y <= ScrollContainer.Size.Y))
        {
            minimumSize = Vector2.Zero;
        }
        TextureRect.CustomMinimumSize = minimumSize;
    }
}
