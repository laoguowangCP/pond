using System;
using Godot;
using LGWCP.Nice;
using LGWCP.Nice.Godot;

namespace LGWCP.Gameplay;

[GlobalClass]
public partial class ComponentResExt4X : ComponentResource
{
    public override Type ComponentType => typeof(ComponentResExt4X);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export] protected ComponentResource Component00;
    [Export] protected ComponentResource Component01;
    [Export] protected ComponentResource Component02;
    [Export] protected ComponentResource Component03;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        if (Component00 != null)
        {
            holder.TryAddComponent(Component00);
        }
        if (Component01 != null)
        {
            holder.TryAddComponent(Component01);
        }
        if (Component02 != null)
        {
            holder.TryAddComponent(Component02);
        }
        if (Component03 != null)
        {
            holder.TryAddComponent(Component03);
        }
        return false;
    }
}