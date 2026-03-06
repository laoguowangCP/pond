using Godot;
using System;
using LGWCP.NiceGD;
using System.Threading.Tasks;
using GodotTask;
using LGWCP.Util.WinApiNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class PhotoStickerPannerView : ComponentResource
{
    public override Type ComponentType => typeof(PhotoStickerPannerView);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public TextureRect TextureRect;
    public ScrollContainer ScrollContainer;
    protected OnStickerSizeChanged OnStickerSizeChanged;

    protected float ZoomLevel = 1f;
    protected float MaxZoomLevel = 10f;
    protected bool IsPanning = false;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<TextureRect>(Name.NP_PhotoStickerTextureRect, out TextureRect);
        TextureRect.GuiInput += OnGuiInput;
        // TextureRect.MouseEntered += OnMouseEntered;
        // TextureRect.MouseExited += OnMouseExited;
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
        // TextureRect.MouseEntered -= OnMouseEntered;
        // TextureRect.MouseExited -= OnMouseExited;
        OnStickerSizeChanged.StickerSizeChanged -= CheckShouldResetMinimumSize;
        return base.OnHolderTryRemove();
    }



    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (Input.IsActionJustReleased(Name.SN_CtrlScrollUp, true))
            {
                ChangeZoomLevel(1.1f);
                TextureRect.AcceptEvent();
                // TextureRect.GetViewport().SetInputAsHandled();
            }
            else if (Input.IsActionJustReleased(Name.SN_CtrlScrollDown, true))
            {
                ChangeZoomLevel(1f/1.1f);
                TextureRect.AcceptEvent();
                // TextureRect.GetViewport().SetInputAsHandled();
            }
            else if (Input.IsActionJustReleased(Name.SN_ShiftScrollUp, true))
            {
                ScrollContainer.ScrollHorizontal -= 32;
            }
            else if (Input.IsActionJustReleased(Name.SN_ShiftScrollDown, true))
            {
                ScrollContainer.ScrollHorizontal += 32;
            }
            else if (Input.IsActionJustReleased(Name.SN_ScrollUp, true))
            {
                ScrollContainer.ScrollVertical -= 32;
            }
            else if (Input.IsActionJustReleased(Name.SN_ScrollDown, true))
            {
                ScrollContainer.ScrollVertical += 32;
            }

            if (@event is InputEventMouseButton mouseButton
                && mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.ShiftPressed && mouseButton.IsPressed())
                {
                    if (Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
                    {
                        DragDropUtil.StartDragDrop(loadImage.ImageFile);
                    }
                }
                else if (mouseButton.CtrlPressed && mouseButton.IsPressed())
                {
                    if (Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
                    {
                        if (mouseButton.AltPressed)
                        {
                            OS.ShellShowInFileManager(loadImage.ImageFile);
                        }
                        else
                        {
                            OS.ShellOpen(loadImage.ImageFile);
                        }
                    }
                }
                else if (mouseButton.IsPressed() && mouseButton.DoubleClick)
                {
                    IsPanning = false;
                    if (GetZoomLevel() > 1.001f)
                    {
                        ChangeZoomLevel(0f);
                    }
                    else
                    {
                        ChangeZoomLevel(1.5f);
                    }
                }
                else if (mouseButton.IsPressed())
                {
                    IsPanning = true;
                }
                else if (mouseButton.IsReleased())
                {
                    IsPanning = false;
                }
            }

            if (IsPanning
                && @event is InputEventMouseMotion mouseMotion)
            {
                Vector2 panMove = mouseMotion.Relative;
                ScrollContainer.ScrollHorizontal -= Mathf.RoundToInt(panMove.X);
                ScrollContainer.ScrollVertical -= Mathf.RoundToInt(panMove.Y);
            }
        }
    }

    protected float GetZoomLevel()
    {
        Vector2 rectSize = TextureRect.Size;
        // Vector2 textureSize = TextureRect.Texture.GetSize();
        Vector2 scrollSize = ScrollContainer.Size;
        return MathF.Max(rectSize.X / scrollSize.X, rectSize.Y / scrollSize.Y);
    }

    protected void ChangeZoomLevel(float level)
    {
        TextureRect.UpdateMinimumSize();
        ScrollContainer.GetCombinedMinimumSize();
        ScrollContainer.DrawFocusBorder = true;
        ScrollContainer.DrawFocusBorder = false;

        float currentZoomLevel = GetZoomLevel();
        if (currentZoomLevel * level > MaxZoomLevel)
        {
            level = MaxZoomLevel / currentZoomLevel;
        }

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
        Vector2 mousePosPanner = ScrollContainer.GetLocalMousePosition();
        Vector2 mousePosTexture = TextureRect.GetLocalMousePosition();
        Vector2 ratioPos = mousePosTexture / customMinimumSize;
        // ratioPos = ratioPos.Clamp(Vector2.Zero, textureSize);
        Vector2 nextMousePosTexture = ratioPos * nextMinimumSize;
        Vector2 nextScroll = nextMousePosTexture - mousePosPanner;
        
        TextureRect.CustomMinimumSize = nextMinimumSize;
        TextureRect.UpdateMinimumSize();
        ScrollContainer.GetCombinedMinimumSize();
        ScrollContainer.DrawFocusBorder = true;
        ScrollContainer.DrawFocusBorder = false;
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
            ScrollContainer.ScrollHorizontal = Mathf.RoundToInt(Mathf.Clamp(nextScroll.X, hScrollBar.MinValue, hScrollBar.MaxValue));
            ScrollContainer.ScrollVertical = Mathf.RoundToInt(Mathf.Clamp(nextScroll.Y, vScrollBar.MinValue, vScrollBar.MaxValue));
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

    public void SetPannerViewInit(Vector2 rectCustomMinimumSize, int hScroll, int vScroll)
    {
        Callable.From(DeferedSetPannerViewInit).CallDeferred();
        void DeferedSetPannerViewInit()
        {
            TextureRect.CustomMinimumSize = rectCustomMinimumSize;
            TextureRect.UpdateMinimumSize();
            ScrollContainer.GetCombinedMinimumSize();
            ScrollContainer.DrawFocusBorder = true;
            ScrollContainer.DrawFocusBorder = false;
            ScrollContainer.ScrollHorizontal = hScroll;
            ScrollContainer.ScrollVertical = vScroll;
        }
    }
}
