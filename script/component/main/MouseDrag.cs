using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class MouseDrag : ComponentResource
{
    public override Type ComponentType => typeof(MouseDrag);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => true;

    public bool IsDragging { get; protected set; } = false;
    public Node2D DragEntity { get; protected set; } = null;
    public IComponent DragHandle { get; protected set; } = null;

    public bool IsDeleteOnDragEnd = false;

    public override void OnEntityReady()
    {
        // Hook when window unfocus, exit current drag.
        var window = Holder.GetTree().Root.GetWindow();
        // window.FocusEntered += OnWindowFocusEntered;
        window.FocusExited += OnWindowFocusExited;

        // Not dragging.
        // Holder.BlockByTag(TagEnum.StickerDragging);
    }

    public override bool OnHolderTryRemove()
    {
        // Unhook when window unfocus.
        var window = Holder.GetWindow();
        window.FocusExited -= OnWindowFocusExited;
        return base.OnHolderTryRemove();
    }

    public bool RequestDragging(Node2D draggingEntity, IComponent dragHandle, bool isManDelete = false)
    {
        if (IsDragging)
        {
            return false;
        }

        // Double check window focus
        if (!Holder.GetTree().Root.GetWindow().HasFocus())
        {
            return false;
        }

        IsDragging = true;
        DragEntity = draggingEntity;
        DragHandle = dragHandle;

        Holder.BlockByTag(TagEnum.FgHover);
        Holder.UnblockByTag(TagEnum.ShowDragArea);
        if (isManDelete)
        {
            Holder.UnblockByTag(TagEnum.StickerDragging);
        }
        return true;
    }

    public bool UnrequestDragging(Node2D dragEntity, bool isManDelete = false)
    {
        // Only unregist itself
        if (DragEntity != dragEntity)
        {
            return false;
        }

        IsDragging = false;
        DragEntity = null;
        DragHandle = null;

        Holder.UnblockByTag(TagEnum.FgHover);
        Holder.BlockByTag(TagEnum.ShowDragArea);
        if (isManDelete)
        {
            if (IsDeleteOnDragEnd)
            {
                if (Holder.TryGetComponent<HandleStickerInSceneTree>(out var handleSticker))
                {
                    handleSticker.RemoveSticker(dragEntity);
                    IsDeleteOnDragEnd = false;
                }
            }
            Holder.BlockByTag(TagEnum.StickerDragging);
        }
        return true;
    }

    public void OnWindowFocusExited()
    {
        // GD.Print("MouseDrag: window unfocus detected!");
        if (DragEntity != null)
        {
            if (DragEntity.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited))
            {
                dragEndOnFocusExited.OnFocusExited();
            }
        }
    }
}
