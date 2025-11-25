using System;
using Godot;
using LGWCP.Nice;
using LGWCP.Nice.Godot;

namespace LGWCP.Gameplay;

[GlobalClass]
public partial class ComponentResExt16X : ComponentResource
{
    public override Type ComponentType => typeof(ComponentResExt16X);
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
    [Export] protected ComponentResource Component08;
    [Export] protected ComponentResource Component09;
    [Export] protected ComponentResource Component10;
    [Export] protected ComponentResource Component11;
    [Export] protected ComponentResource Component12;
    [Export] protected ComponentResource Component13;
    [Export] protected ComponentResource Component14;
    [Export] protected ComponentResource Component15;

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
        if (Component08 != null)
        {
            holder.TryAddComponent(Component08);
        }
        if (Component09 != null)
        {
            holder.TryAddComponent(Component09);
        }
        if (Component10 != null)
        {
            holder.TryAddComponent(Component10);
        }
        if (Component11 != null)
        {
            holder.TryAddComponent(Component11);
        }
        if (Component12 != null)
        {
            holder.TryAddComponent(Component12);
        }
        if (Component13 != null)
        {
            holder.TryAddComponent(Component13);
        }
        if (Component14 != null)
        {
            holder.TryAddComponent(Component14);
        }
        if (Component15 != null)
        {
            holder.TryAddComponent(Component15);
        }
        return false;
    }
}