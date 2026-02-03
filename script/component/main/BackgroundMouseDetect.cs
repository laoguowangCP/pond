using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BackgroundMouseDetect : ComponentResource
{
    public override Type ComponentType => typeof(BackgroundMouseDetect);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_BgPanel = "./UICanvasLayer/BackgroundPanel";
    protected Panel BgPanel;
    protected Node2D Entity;
    protected BtnAsDragCreateTip BtnAsDragCreateTip;

    public override void OnEntityReady()
    {
        Holder.TryGetNode<Panel>(NP_BgPanel, out BgPanel);
        BgPanel.GuiInput += OnBgGuiInput;

        Holder.TryGetEntity<Node2D>(out Entity);

        Holder.TryGetComponent<BtnAsDragCreateTip>(out BtnAsDragCreateTip);
    }

    public override bool OnHolderTryRemove()
    {
        BgPanel.GuiInput -= OnBgGuiInput;
        return base.OnHolderTryRemove();
    }

    public void OnBgGuiInput(InputEvent @event)
    {
        GD.Print("Bg panel: gui input");

        Vector2 mousePos = Entity.GetGlobalMousePosition();
        // If mouse near bottom, show create button
        if (IsShowCreateTipBtn(mousePos))
        {
            BtnAsDragCreateTip.ButtonShow();
        }
        else
        {
            BtnAsDragCreateTip.ButtonHide();
        }
    }

    public bool IsShowCreateTipBtn(Vector2 mousePos)
    {
        bool isShow = false;

        // Check in show area
        Vector2I windowSize = Holder.GetWindow().Size;
        int halfX = windowSize.X / 2;
        if (mousePos.X > halfX - 256
            && mousePos.X < halfX + 256
            && mousePos.Y > windowSize.Y - 256)
        {
            isShow = true;
        }

        return isShow;
    }
}