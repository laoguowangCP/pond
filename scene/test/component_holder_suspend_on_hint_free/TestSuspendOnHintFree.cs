using Godot;
using System;
using LGWCP.NiceGD;
using LGWCP.Extension;
using LGWCP.Pond;

// namespace LGWCP.;

[GlobalClass]
[Tool]
public partial class TestSuspendOnHintFree : ComponentResource
{
    public override Type ComponentType => typeof(TestSuspendOnHintFree);
    public override TickGroupEnum TickGroup => TickGroupEnum.Process;
    public override bool IsRegist => false;

    protected Node Entity;
    protected Node ToBeSuspended;
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.TryGetEntity<Node>(out Entity);
        return true;
    }

    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
        // var packed = ResourceLoader.Load<PackedScene>("res://scene/test/component_holder_suspend_on_hint_free/suspend_on_free_entity.tscn");
        // ToBeSuspended = packed.Instantiate<Node>();
        // Entity.AddChild(ToBeSuspended);

        /*
        if (ToBeSuspended.TryGetComponentHolder(out var holder))
        {
            holder.TickGroupSuspend<FooCompToBeSuspended>();
            holder.TickGroupUnsuspend<FooCompToBeSuspended>();
            holder.HintFreeOnExitTree();
            Entity.RemoveChild(ToBeSuspended);
            ToBeSuspended.QueueFree();
        }
        */
    }

    public override void Tick(TickContext ctx)
    {
        // Do tick
        var packed = ResourceLoader.Load<PackedScene>("res://scene/test/component_holder_suspend_on_hint_free/suspend_on_free_entity.tscn");
        ToBeSuspended = packed.Instantiate<Node>();
        Entity.AddChild(ToBeSuspended);

        if (ToBeSuspended != null
            && ToBeSuspended.TryGetComponentHolder(out var holder))
        {
            holder.TickGroupSuspend<FooCompToBeSuspended>();
            holder.TickGroupUnsuspend<FooCompToBeSuspended>();
            holder.HintFreeOnExitTree();
            Entity.RemoveChild(ToBeSuspended);
            ToBeSuspended.QueueFree();
            ToBeSuspended = null;
        }
    }

    /*
    public override bool OnHolderTryRemove()
    {
        return base.OnHolderTryRemove();
        // Simply return true, or custom recycle component.
        // Holder = null; return true;
    }
    */

    /*
    public override void OnActivated() {}
    public override void OnDeactivated() {}

    public override bool ShouldDeactivate()
    {
        return false;
    }

    public override bool ShouldActivate()
    {
        // If not blocked, always activated.
        return true;
    }
    */
}

