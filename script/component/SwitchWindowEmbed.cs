using System;
using Godot;
using System.Runtime.InteropServices;
using GodotTask;
using LGWCP.Nice;
using LGWCP.Nice.Godot;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SwitchWindowEmbed : ComponentResource
{
    public override Type ComponentType => typeof(SwitchWindowEmbed);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export]
    protected NodePath NP_SwitchWindowEmbedBtn = "../SwitchWindowEmbedBtn";
    protected Button SwitchWindowEmbedBtn;

    protected WindowEmbedDesktop WindowEmbedDesktop;

    public override void OnEntityReady()
    {
        Holder.TryGetComponent(out WindowEmbedDesktop);

        SwitchWindowEmbedBtn = Holder.GetNodeOrNull<Button>(NP_SwitchWindowEmbedBtn);
        SwitchWindowEmbedBtn.Pressed += OnSwitchWindowEmbedBtnPressed;
    }

    public void OnSwitchWindowEmbedBtnPressed()
    {
        if (WindowEmbedDesktop.IsEmbeded)
        {
            // Embed
            WindowEmbedDesktop.UnEmbed();
        }
        else 
        {
            // Embed
            WindowEmbedDesktop.Embed();
        }
    }
}
