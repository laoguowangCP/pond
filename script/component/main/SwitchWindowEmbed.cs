using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SwitchWindowEmbed : ComponentResource
{
    public override Type ComponentType => typeof(SwitchWindowEmbed);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    // [Export]
    // protected NodePath NP_SwitchWindowEmbedBtn = "../SwitchWindowEmbedBtn";
    // protected Button SwitchWindowEmbedBtn;

}
