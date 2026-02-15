using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class ForegroundDragDelete : ComponentResource
{
    public override Type ComponentType => typeof(ForegroundDragDelete);
    public override TickGroupEnum TickGroup => TickGroupEnum.Input;
    public override bool IsRegist => false;

    [Export] protected NodePath NP_CtrlAsDragDelete = "./UICanvasLayer/DragDeleteButton";
    protected Control CtrlAsDragDelete;
    protected MouseDrag MouseDrag;

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
        Holder.TryGetComponent<MouseDrag>(out MouseDrag);
        CtrlAsDragDelete.Visible = false;
    }

    public override void Tick(TickContext ctx)
    {
        // Check if dragging to delele button area
        // GD.Print(CtrlAsDragDelete.Position);
        // GD.Print(CtrlAsDragDelete.GlobalPosition);
        // GD.Print(CtrlAsDragDelete.GetRect());
        var mousePos = CtrlAsDragDelete.GetGlobalMousePosition();
        var ctrlPos = CtrlAsDragDelete.GlobalPosition;
        var ctrlSize = CtrlAsDragDelete.GetRect().Size;

        /*
        if (mousePos.X >= ctrlPos.X
            && mousePos.X <= ctrlPos.X + ctrlSize.X
            && mousePos.Y >= ctrlPos.Y
            && mousePos.Y <= ctrlPos.Y + ctrlSize.Y)
        */
        if (mousePos.Y >= ctrlPos.Y)
        {
            // Inside delete area
            MouseDrag.IsDeleteOnDragEnd = true;
        }
        else
        {
            MouseDrag.IsDeleteOnDragEnd = false;
        }
    }
}