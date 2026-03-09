using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class FooCompToBeSuspended : ComponentResource
{
    public override Type ComponentType => typeof(FooCompToBeSuspended);
    public override TickGroupEnum TickGroup => TickGroupEnum.Process;
    public override bool IsRegist => false;
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.TryAddTag(this, TagEnum.FgHover);
        return true;
    }

    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
    }

    public override void Tick(TickContext ctx)
    {
        // Do tick
        GD.Print($"{ComponentType} ticks.");
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

