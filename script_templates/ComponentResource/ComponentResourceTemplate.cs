using _BINDINGS_NAMESPACE_;
using System;
using LGWCP.NiceGD;

// namespace LGWCP.;

[GlobalClass]
[Tool]
public partial class _CLASS_ : ComponentResource
{
    public override Type ComponentType => typeof(_CLASS_);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        return true;
    }

    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
    }

    /*
    public override void Tick(TickContext ctx)
    {
        // Do tick
    }
    */

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
