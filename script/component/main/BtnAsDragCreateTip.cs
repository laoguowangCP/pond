using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BtnAsDragCreateTip : ComponentResource
{
    public override Type ComponentType => typeof(BtnAsDragCreateTip);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_CreateBtn = "./UICanvasLayer/CreateTipButton";
    protected Button CreateBtn;
    protected Node2D Entity;
    protected bool IsDraggingNewTip = false;
    protected Node2D TipDragging;
    protected Vector2 DragBeginDisplacement;
    protected DragArea DragArea;

    public override void OnEntityReady()
    {
        Holder.TryGetNode<Button>(NP_CreateBtn, out CreateBtn);
        Holder.TryGetEntity<Node2D>(out Entity);
        CreateBtn.FocusMode = Control.FocusModeEnum.None;
        ButtonHide();
        CreateBtn.GuiInput += OnGuiInput;
        CreateBtn.ButtonDown += OnButtonDown;
        CreateBtn.ButtonUp += OnButtonUp;

        Holder.TryGetComponent<DragArea>(out DragArea);
    }

    public override bool OnHolderTryRemove()
    {
        CreateBtn.GuiInput -= OnGuiInput;
        CreateBtn.ButtonDown -= OnButtonDown;
        CreateBtn.ButtonUp -= OnButtonUp;
        return base.OnHolderTryRemove();
    }

    protected void OnButtonDown()
    {
        // 1. Create tip sticker
        if (!Holder.TryGetComponent<StickerBuilder>(out var stickerBuilder))
        {
            return;
        }
        var tip = stickerBuilder.Build();
        stickerBuilder.StickerCanvasLayer.AddChild(tip);

        // 1.5. Let sticker follow mouse
        Vector2 mousePos = Entity.GetGlobalMousePosition();
        tip.TryGetComponent<StickerControlSize>(out var stickerControlSize);
        DragBeginDisplacement = new Vector2(stickerControlSize.Size.X / 2f, 0f);
        tip.GlobalPosition = mousePos - DragBeginDisplacement;

        // 2. Hide button
        ButtonHide();

        // 3. Hint dragging mode
        IsDraggingNewTip = true;
        TipDragging = tip;

        // 4. Disable delete?

    }

    protected void OnButtonUp()
    {
        IsDraggingNewTip = false;
        TipDragging = null;
    }

    protected void OnGuiInput(InputEvent @event)
    {
        if (IsDraggingNewTip)
        {
            Vector2 mousePos = Entity.GetGlobalMousePosition();
            TipDragging.GlobalPosition = DragArea.GetGlobalPositionRegulated(mousePos) - DragBeginDisplacement;
        }
    }

    public void ButtonShow()
    {
        CreateBtn.Text = "按住并拖动";
        CreateBtn.Modulate = new Color(0xffffffff);
    }

    public void ButtonHide()
    {
        CreateBtn.Text = "";
        CreateBtn.Modulate = new Color(0xffffff00);
    }
}

