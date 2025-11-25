using System;
using Godot;
using LGWCP.Nice;
using LGWCP.Nice.Godot;

namespace LGWCP.Gameplay;

[GlobalClass]
public partial class ComponentResExt8X : ComponentResource
{
    public override Type ComponentType => typeof(ComponentResExt8X);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export] protected ComponentResource Component00;
    [Export] protected ComponentResource Component01;
    [Export] protected ComponentResource Component02;
    [Export] protected ComponentResource Component03;
    [Export] protected ComponentResource Component04;
    [Export] protected ComponentResource Component05;
    [Export] protected ComponentResource Component06;
    [Export] protected ComponentResource Component07;

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
        if (Component04 != null)
        {
            holder.TryAddComponent(Component04);
        }
        if (Component05 != null)
        {
            holder.TryAddComponent(Component05);
        }
        if (Component06 != null)
        {
            holder.TryAddComponent(Component06);
        }
        if (Component07 != null)
        {
            holder.TryAddComponent(Component07);
        }
        return false;
    }
}