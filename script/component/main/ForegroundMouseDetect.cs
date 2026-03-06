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


    protected Node2D Entity;
    protected MouseDrag MouseDrag;
    protected BtnAsDragCreateTip BtnAsDragCreateTip;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        Holder.TryGetEntity<Node2D>(out Entity);
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.FgHover);
        return true;
    }

    public override void OnEntityReady()
    {
        // Holder.TryGetNode<Panel>(NP_FgPanel, out FgPanel);
        // FgPanel.GuiInput += OnFgGuiInput;
        Holder.TryGetComponent<MouseDrag>(out MouseDrag);
        Holder.TryGetComponent<BtnAsDragCreateTip>(out BtnAsDragCreateTip);
    }

    public override bool OnHolderTryRemove()
    {
        // FgPanel.GuiInput -= OnFgGuiInput;
        return base.OnHolderTryRemove();
    }
    
    public override void OnActivated()
    {
        // BtnAsDragCreateTip.ButtonEnable();
    }
    public override void OnDeactivated()
    {
        // BtnAsDragCreateTip.ButtonDisable();
    }

    public override void Tick(TickContext ctx)
    {
        // InputEvent @event = ctx.AnyInput;
        // if (@event is InputEventMouseMotion)
        {
            Vector2 mousePos = Entity.GetGlobalMousePosition();
            // If mouse near bottom, show create button
            if (IsEnableCreateTipBtn(mousePos))
            {
                BtnAsDragCreateTip.ButtonShow();
                // BtnAsDragCreateTip.ButtonEnable();
            }
            else
            {
                BtnAsDragCreateTip.ButtonHide();
            }
        }
    }

    public bool IsEnableCreateTipBtn(Vector2 mousePos)
    {
        bool isShow;

        // Check in show area
        /*
        Vector2I windowSize = Holder.GetWindow().Size;
        int halfX = windowSize.X / 2;
        if (mousePos.X > halfX - 256
            && mousePos.X < halfX + 256
            && mousePos.Y > windowSize.Y - 256)
        {
            isShow = true;
        }
        */

        var createBtn = BtnAsDragCreateTip.GetCreateBtn;
        var ctrlPos = createBtn.GlobalPosition;
        var ctrlSize = createBtn.GetRect().Size;

        if (mousePos.X >= ctrlPos.X
            && mousePos.X <= ctrlPos.X + ctrlSize.X
            && mousePos.Y >= ctrlPos.Y
            && mousePos.Y <= ctrlPos.Y + ctrlSize.Y)
        {
            // Inside delete area
            isShow = true;
        }
        else
        {
            isShow = false;
        }

        return isShow;
    }

}
