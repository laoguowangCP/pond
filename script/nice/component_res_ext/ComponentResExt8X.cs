using System;
using Godot;
using LGWCP.Nice;
using LGWCP.Nice.Godot;

namespace LGWCP.Nice.Godot;

[GlobalClass]
[Tool]
public partial class ComponentResExt8X : ComponentResource, IComponentSheet
{
    public override Type ComponentType => typeof(ComponentResExt8X);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export] public ComponentResource Component00;
    [Export] public ComponentResource Component01;
    [Export] public ComponentResource Component02;
    [Export] public ComponentResource Component03;
    [Export] public ComponentResource Component04;
    [Export] public ComponentResource Component05;
    [Export] public ComponentResource Component06;
    [Export] public ComponentResource Component07;

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
            Component07
        ];
    }
}