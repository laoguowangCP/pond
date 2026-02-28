using Godot;
using System;
using LGWCP.NiceGD;

// namespace LGWCP.;

[GlobalClass]
[Tool]
public partial class OnStickerSizeChanged : ComponentResource
{
    public override Type ComponentType => typeof(OnStickerSizeChanged);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public delegate void StickerSizeChangedHandler();
    public event StickerSizeChangedHandler StickerSizeChanged;

    public void Trigger()
    {
        StickerSizeChanged?.Invoke();
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
        Holder = null;
        // Simply return true.
        // Or remove other component you want.
        return true;
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

