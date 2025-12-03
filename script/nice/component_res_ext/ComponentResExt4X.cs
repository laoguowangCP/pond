using System;
using Godot;
using LGWCP.Nice;
using LGWCP.Nice.Godot;

namespace LGWCP.Nice;

[GlobalClass]
[Tool]
public partial class ComponentResExt4X : ComponentResource, IComponentSheet
{
    public override Type ComponentType => typeof(ComponentResExt4X);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export] public ComponentResource Component00;
    [Export] public ComponentResource Component01;
    [Export] public ComponentResource Component02;
    [Export] public ComponentResource Component03;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        holder.TryAddComponent(Component00);
        holder.TryAddComponent(Component01);
        holder.TryAddComponent(Component02);
        holder.TryAddComponent(Component03);
        return false;
    }

    public IComponent[] GetSheetComponents()
    {
        return [
            Component00,
            Component01,
            Component02,
            Component03
        ];
    }
}