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

    public static readonly NodePath NP_CreateBtn = "./UICanvasLayer/BackgroundPanel/GridContainer/PanelD/CreateTipButton";
    protected Button CreateBtn;
    protected Node2D Entity;
    public bool IsDraggingNewTip { get; protected set; } = false;
    public bool IsTryCreateNewTip { get; protected set; } = false;
    protected Node2D TipDragging;
    protected Vector2 DragBeginDisplacement;
    protected DragArea DragArea;

    public override void OnEntityReady()
    {
        Holder.TryGetNode<Button>(NP_CreateBtn, out CreateBtn);
        CreateBtn.Text = "按住并拖动";

        Holder.TryGetEntity<Node2D>(out Entity);
        CreateBtn.FocusMode = Control.FocusModeEnum.None;
        ButtonDisable();
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
        // 0. Hint trying create new tip
        IsTryCreateNewTip = true;
        
        // 1. Show drag out hint

        // 2. Hide button
        ButtonHide();

        // 4. Block tag
        Holder.BlockByTag(TagEnum.FgHover);
    }

    protected void OnButtonUp()
    {
        IsDraggingNewTip = false;
        TipDragging = null;
        // ButtonShow();
        Holder.UnblockByTag(TagEnum.FgHover);
    }

    protected void OnGuiInput(InputEvent @event)
    {
        if (IsTryCreateNewTip)
        {
            // if (CheckLeaveDragArea) // create tip
            {
                // 1. Create tip sticker
                if (!Holder.TryGetComponent<StickerBuilder>(out var stickerBuilder)
                    || !Holder.TryGetComponent<HandleStickerInSceneTree>(out var handleStickerInSceneTree))
                {
                    return;
                }
                var tip = stickerBuilder.Build();
                handleStickerInSceneTree.AddSticker(tip);

                // 1.5. Let sticker follow mouse
                Vector2 mousePos = Entity.GetGlobalMousePosition();
                tip.TryGetComponent<StickerControlSize>(out var stickerControlSize);
                DragBeginDisplacement = new Vector2(stickerControlSize.Size.X / 2f, 0f);
                tip.GlobalPosition = mousePos - DragBeginDisplacement;

                // 2. Hide drag out hint

                // 3. Hint dragging mode
                IsDraggingNewTip = true;
                TipDragging = tip;

                IsTryCreateNewTip = false;
            }
        }
        else if (IsDraggingNewTip)
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

    public void ButtonEnable()
    {
        CreateBtn.Visible = true;
        // CreateBtn.Text = "按住并拖动";
        // CreateBtn.Modulate = new Color(0xffffffff);
    }

    public void ButtonDisable()
    {
        CreateBtn.Visible = false;
        // CreateBtn.Text = "";
        // CreateBtn.Modulate = new Color(0xffffff00);
    }
}

