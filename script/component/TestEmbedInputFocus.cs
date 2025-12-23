using System;
using Godot;
using System.Runtime.InteropServices;
using GodotTask;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class TestEmbedInputFocus : ComponentResource
{
    public override Type ComponentType => typeof(TestEmbedInputFocus);
    public override TickGroupEnum TickGroup => TickGroupEnum.Input;
    public override bool IsRegist => false;

    public override void Tick(TickContext ctx)
    {
        GD.Print("Has input");
    }
}
