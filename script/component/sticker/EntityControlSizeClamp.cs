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
    public Vector2 MinSize = new Vector2(100, 50);
    [Export]
    public Vector2 MaxSize = new Vector2(1024, 1024);

    public Vector2 GetClampedSize(Vector2 size) => size.Clamp(MinSize, MaxSize);
}

