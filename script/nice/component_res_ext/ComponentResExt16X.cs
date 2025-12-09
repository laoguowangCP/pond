using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.NiceGD;

[GlobalClass]
[Tool]
public partial class ComponentResExt16X : ComponentResource, IComponentSheet
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
        holder.TryAddComponent(Component00);
        holder.TryAddComponent(Component01);
        holder.TryAddComponent(Component02);
        holder.TryAddComponent(Component03);
        holder.TryAddComponent(Component04);
        holder.TryAddComponent(Component05);
        holder.TryAddComponent(Component06);
        holder.TryAddComponent(Component07);
        holder.TryAddComponent(Component08);
        holder.TryAddComponent(Component09);
        holder.TryAddComponent(Component10);
        holder.TryAddComponent(Component11);
        holder.TryAddComponent(Component12);
        holder.TryAddComponent(Component13);
        holder.TryAddComponent(Component14);
        holder.TryAddComponent(Component15);
        return false;
    }

    public IComponent[] GetSheetComponents()
    {
        return [
            Component00,
            Component01,
            Component02,
            Component03,
            Component04,
            Component05,
            Component06,
            Component07,
            Component08,
            Component09,
            Component10,
            Component11,
            Component12,
            Component13,
            Component14,
            Component15
        ];
    }
}