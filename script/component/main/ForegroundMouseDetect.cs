using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class ForegroundMouseDetect : ComponentResource
{
    public override Type ComponentType => typeof(ForegroundMouseDetect);
    public override TickGroupEnum TickGroup => TickGroupEnum.Input;
    public override bool IsRegist => false;

    // public static readonly NodePath NP_FgPanel = "./UICanvasLayer/BackgroundPanel/ForegroundPanel";
    // protected Panel FgPanel;
    protected Node2D Entity;
    protected MouseDrag MouseDrag;
    protected BtnAsDragCreateTip BtnAsDragCreateTip;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.FgHover);
        return true;
    }

    public override void OnEntityReady()
    {
        // Holder.TryGetNode<Panel>(NP_FgPanel, out FgPanel);
        // FgPanel.GuiInput += OnFgGuiInput;

        Holder.TryGetEntity<Node2D>(out Entity);

        Holder.TryGetComponent<MouseDrag>(out MouseDrag);
        Holder.TryGetComponent<BtnAsDragCreateTip>(out BtnAsDragCreateTip);
    }

    public override bool OnHolderTryRemove()
    {
        // FgPanel.GuiInput -= OnFgGuiInput;
        return base.OnHolderTryRemove();
    }

    public override void Tick(TickContext ctx)
    {
        // GD.Print("Fg input: ", BlockCount);
        InputEvent @event = ctx.AnyInput;
        if (@event is InputEventMouseMotion)
        {
            Vector2 mousePos = Entity.GetGlobalMousePosition();
            // If mouse near bottom, show create button
            if (IsEnableCreateTipBtn(mousePos))
            {
                BtnAsDragCreateTip.ButtonShow();
                BtnAsDragCreateTip.ButtonEnable();
            }
            else
            {
                BtnAsDragCreateTip.ButtonDisable();
            }
        }
    }

    public bool IsEnableCreateTipBtn(Vector2 mousePos)
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
