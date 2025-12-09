using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class EntityControlSizeClamp : ComponentResource
{
    public override Type ComponentType => typeof(EntityControlSizeClamp);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export]
    public Vector2 MinSize = new Vector2(128, 128);
    [Export]
    public Vector2 MaxSize = new Vector2(1024, 1024);
}

