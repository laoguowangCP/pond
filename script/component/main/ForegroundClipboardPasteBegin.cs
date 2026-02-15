using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class ForegroundClipboardPasteBegin : ComponentResource
{
    public override Type ComponentType => typeof(ForegroundClipboardPasteBegin);
    public override TickGroupEnum TickGroup => TickGroupEnum.Input;
    public override bool IsRegist => false;

    protected Node2D Entity;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        Holder.TryGetEntity<Node2D>(out Entity);
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.FgHover);
        return true;
    }

}
