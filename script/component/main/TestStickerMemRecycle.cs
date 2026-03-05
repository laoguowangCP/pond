using Godot;
using System;
using LGWCP.NiceGD;
using LGWCP.Pond;

// namespace LGWCP.;

[GlobalClass]
[Tool]
public partial class TestStickerMemRecycle : ComponentResource
{
    public override Type ComponentType => typeof(TestStickerMemRecycle);
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

    public override void Tick(TickContext ctx)
    {
        if (Holder.TryGetComponent<StickerBuilder>(out var builder)
            && Holder.TryGetComponent<HandleStickerInSceneTree>(out var handleSticker))
        {
            builder.BuildStickerTip(String.Empty, out var sticker);
            handleSticker.RemoveSticker(sticker);
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

