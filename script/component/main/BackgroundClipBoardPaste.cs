using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BackgroundClipBoardPaste : ComponentResource
{
    public override Type ComponentType => typeof(BackgroundClipBoardPaste);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_BgPanel = "./UICanvasLayer/BackgroundPanel/GridContainer/PanelMain";
    protected Control BgPanel;
    protected Node2D Entity;
    protected DragArea DragArea;
    protected StickerBuilder StickerBuilder;

    protected StringName SN_UiPaste = "ui_paste";
    protected StringName SN_MouseLeft = "mouse_left";

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        Holder.TryGetEntity<Node2D>(out Entity);
        Holder.TryGetNodeFromEntity<Control>(NP_BgPanel, out BgPanel);
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.FgHover);
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<DragArea>(out DragArea);
        Holder.TryGetComponent<StickerBuilder>(out StickerBuilder);
        BgPanel.GuiInput += OnGuiInput;
        // BgPanel.FocusEntered += OnFocusEntered;
        // BgPanel.FocusExited += OnFocusExited;
    }

    public override bool OnHolderTryRemove()
    {
        BgPanel.GuiInput -= OnGuiInput;

        return base.OnHolderTryRemove();
    }

    private void OnFocusEntered()
    {
        GD.Print("Background panel focus enter.");
        Holder.UnblockByTag(TagEnum.ShowDragArea);
    }

    private void OnFocusExited()
    {
        GD.Print("Background panel focus exit.");
        Holder.BlockByTag(TagEnum.ShowDragArea);
    }


    protected void OnGuiInput(InputEvent @event)
    {
        /*
        if (Input.IsActionJustPressed("mouse_left"))
        {
            GD.Print("BgPanel.GrabFocus");
            BgPanel.GrabFocus();
        }
        */


        // Paste command
        if (Input.IsActionJustPressed(SN_UiPaste)
            && Input.IsActionPressed(SN_MouseLeft))
        {
            GD.Print("Background ui paste");
            Vector2 mousePos = Entity.GetGlobalMousePosition();
            // if (DragArea.CheckAvailableDragArea(mousePos))
            
            // Start paste from clip board
        }
    }

    // protected void OnFocusEntered
}
