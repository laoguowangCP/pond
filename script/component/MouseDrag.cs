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

    public Node2D DragEntity { get; protected set; } = null;
    public IComponent DragHandle { get; protected set; } = null;

    public override void OnEntityReady()
    {
        // Hook when window unfocus, exit current drag.
        var window = Holder.GetTree().Root.GetWindow();
        // window.FocusEntered += OnWindowFocusEntered;
        window.FocusExited += OnWindowFocusExited;
    }

    public override bool OnHolderTryRemove()
    {
        // Unhook when window unfocus.
        var window = Holder.GetWindow();
        window.FocusExited -= OnWindowFocusExited;
        return base.OnHolderTryRemove();
    }

    public bool RequestDragging(Node2D draggingEntity, IComponent dragHandle)
    {
        if (DragEntity != null)
        {
            return false;
        }

        // Double check window focus
        if (!Holder.GetTree().Root.GetWindow().HasFocus())
        {
            return false;
        }

        DragEntity = draggingEntity;
        DragHandle = dragHandle;
        return true;
    }

    public bool UnrequestDragging(Node2D dragEntity)
    {
        // Only unregist itself
        if (DragEntity != dragEntity)
        {
            return false;
        }

        DragEntity = null;
        DragHandle = null;
        return true;
    }

    public void OnWindowFocusExited()
    {
        GD.Print("MouseDrag: window unfocus detected!");
        if (DragEntity != null)
        {
            if (DragEntity.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited))
            {
                dragEndOnFocusExited.OnFocusExited();
            }
        }
    }
}
