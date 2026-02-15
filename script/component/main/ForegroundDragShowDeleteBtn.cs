using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class ForegroundDragShowDeleteBtn : ComponentResource
{
    public override Type ComponentType => typeof(ForegroundDragShowDeleteBtn);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;
    
    [Export] protected NodePath NP_CtrlAsDragDelete = "./UICanvasLayer/DragDeleteButton";
    protected Control CtrlAsDragDelete;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.StickerDragging);

        Holder.TryGetNodeFromEntity<Control>(NP_CtrlAsDragDelete, out CtrlAsDragDelete);
        return true;
    }

    public override void OnEntityReady()
    {
        CtrlAsDragDelete.Visible = false;
    }

    public override void OnActivated()
    {
        CtrlAsDragDelete.Visible = true;
    }
    public override void OnDeactivated()
    {
        CtrlAsDragDelete.Visible = false;
    }
}
