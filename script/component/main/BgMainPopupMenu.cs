using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BgMainPopupMenu : ComponentResource
{
    public override Type ComponentType => typeof(BgMainPopupMenu);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export]
    protected NodePath NP_PopupMenu;
    protected PopupMenu PopupMenu;

    [Export]
    protected NodePath NP_BgPanel;
    protected PanelContainer BgPanel;
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.TryGetNode<PopupMenu>(NP_PopupMenu, out PopupMenu);
        Holder.TryGetNode<PanelContainer>(NP_BgPanel, out BgPanel);
        BgPanel.GuiInput += OnGuiInput;
        return true;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Right
                && mouseButton.IsReleased())
            {
                // PopupMenu.Visible = true;
                Vector2 mousePos = BgPanel.GetGlobalMousePosition();
                PopupMenu.Popup(new Rect2I(Mathf.RoundToInt(mousePos.X), Mathf.RoundToInt(mousePos.Y), 0, 0));
                // PopupMenu.Popup();
            }
        }
    }


    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
    }

    /*
    public override void Tick(TickContext ctx)
    {
        // Do tick
    }
    */

    /*
    public override bool OnHolderTryRemove()
    {
        return base.OnHolderTryRemove();
        // Simply return true, or custom recycle component.
        // Holder = null; return true;
    }
    */

    /*
    public override void OnActivated() {}
    public override void OnDeactivated() {}

    public override bool ShouldDeactivate()
    {
        return false;
    }

    public override bool ShouldActivate()
    {
        // If not blocked, always activated.
        return true;
    }
    */
}

