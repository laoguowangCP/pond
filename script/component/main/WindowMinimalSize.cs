using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class WindowMinimalSize : ComponentResource
{
    public override Type ComponentType => typeof(WindowMinimalSize);
    public override TickGroupEnum TickGroup => TickGroupEnum.Input;
    public override bool IsRegist => false;

    [Export] protected Vector2I Size = new(500, 360);

    public override void OnEntityReady()
    {
        var window = Holder.GetWindow();
        window.MinSize = Size;
    }
}