using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BackgroundGrabFocus : ComponentResource
{
    public override Type ComponentType => typeof(BackgroundGrabFocus);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_BgPanel = "./UICanvasLayer/BackgroundPanel/GridContainer/PanelMain";
    protected Control BgPanel;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<Control>(NP_BgPanel, out BgPanel);
        // BgPanel.GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("mouse_left"))
        {
            GD.Print("BgPanel.GrabFocus");
            BgPanel.GrabFocus();
        }
    }
}
