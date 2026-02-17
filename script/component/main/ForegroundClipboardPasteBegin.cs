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
    protected DragArea DragArea;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        Holder.TryGetEntity<Node2D>(out Entity);
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.FgHover);
        return true;
    }

    public override void OnEntityReady()
    {
        // Holder.TryGetNode<Panel>(NP_FgPanel, out FgPanel);
        // FgPanel.GuiInput += OnFgGuiInput;
        Holder.TryGetComponent<DragArea>(out DragArea);
    }

    public override void Tick(TickContext ctx)
    {
        // GD.Print("Foreground input");
    }

}
