using Godot;
using System;
using LGWCP.NiceGD;

// namespace LGWCP.;

[GlobalClass]
[Tool]
public partial class OnTrServerSetLocale : ComponentResource
{
    public override Type ComponentType => typeof(OnTrServerSetLocale);
    public override TickGroupEnum TickGroup => TickGroupEnum.SetLocale;
    public override bool IsRegist => false;
    
    public delegate void TrServerSetLocaleHandler();
    public event TrServerSetLocaleHandler TrServerSetLocale;

    public override void Tick(TickContext ctx)
    {
        TrServerSetLocale?.Invoke();
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

