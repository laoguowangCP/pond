using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BtnAsDragHandle : ComponentResource
{
    public override Type ComponentType => typeof(BtnAsDragHandle);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_HandleBtn = "./EntityControl/DragHandleBtn";
    protected Button HandleBtn;

    protected Node2D Entity;
    public bool IsDragging { get; protected set; }
    public Vector2 DragBeginDisplacement  { get; protected set; } = Vector2.Zero;

    public KeepInDragAreaOnWindowSizeChanged KeepInDragArea;

    protected static readonly StringName SN_MouseLeft = "mouse_left";
    protected static readonly StringName SN_MouseRight = "mouse_right";


    public override void OnEntityReady()
    {
        Holder.TryGetEntity<Node2D>(out Entity);
        Holder.TryGetComponent(out KeepInDragArea);

        Holder.TryGetNodeFromEntity<Button>(NP_HandleBtn, out HandleBtn);
        HandleBtn.FocusMode = Control.FocusModeEnum.None;
        HandleBtn.MouseDefaultCursorShape = Control.CursorShape.PointingHand;

        HandleBtn.ButtonDown += DragBegin;
        HandleBtn.ButtonUp += DragEnd;
        HandleBtn.ButtonUp += KeepInDragArea.KeepInDragArea;
        HandleBtn.GuiInput += OnGuiInput;

        /*
        HandleBtn.MouseEntered += OnMouseEntered;
        HandleBtn.MouseExited += OnMouseExited;
        */

        Holder.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited);
        dragEndOnFocusExited.DragEnd += DragEnd;
    }

    public override bool OnHolderTryRemove()
    {
        Holder.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited);
        HandleBtn.ButtonDown -= DragBegin;
        HandleBtn.ButtonUp -= DragEnd;
        HandleBtn.ButtonUp -= KeepInDragArea.KeepInDragArea;
        HandleBtn.GuiInput -= OnGuiInput;
        dragEndOnFocusExited.DragEnd -= DragEnd;
        
        /*
        HandleBtn.MouseEntered -= OnMouseEntered;
        HandleBtn.MouseExited -= OnMouseExited;
        */
        return base.OnHolderTryRemove();
    }

    public void DragBegin()
    {
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.RequestDragging(Entity, this, isManDelete: true))
            {
                IsDragging = true;
                // Get mouse pos to entity pos
                Vector2 mousePos = Entity.GetGlobalMousePosition();
                Vector2 entityPos = Entity.GlobalPosition;
                DragBeginDisplacement = entityPos - mousePos;
                HandleBtn.FocusMode = Control.FocusModeEnum.Click;
                HandleBtn.GrabFocus();
            }
        }
    }

    public void DragEnd()
    {
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.UnrequestDragging(Entity, isManDelete: true))
            {
                IsDragging = false;
                // Clear displacement
                DragBeginDisplacement = Vector2.Zero;
                // Entity.QueueRedraw();
                HandleBtn.FocusMode = Control.FocusModeEnum.None;
            }
        }
    }

    public void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            // Change node order
            if (Input.IsActionJustPressed(SN_MouseLeft))
            {
                if (Nice.I.TryGetRegistedComponentFirst<HandleStickerInSceneTree>(out var HandleStickerInSceneTree))
                {
                    HandleStickerInSceneTree.StickerMoveToTop(Entity);
                }
            }
            else if (Input.IsActionJustPressed(SN_MouseRight))
            {
                if (Nice.I.TryGetRegistedComponentFirst<HandleStickerInSceneTree>(out var HandleStickerInSceneTree))
                {
                    HandleStickerInSceneTree.StickerMoveToBottom(Entity);
                }
            }

            if (CheckDragging())
            {
                // Update entity pos
                Vector2 mousePos = Entity.GetGlobalMousePosition();
                if (Nice.I.TryGetRegistedComponentFirst<DragArea>(out var dragArea))
                {
                    var mousePosNext = dragArea.GetGlobalPositionSoftRegulated(Entity.GlobalPosition - DragBeginDisplacement, mousePos);
                    Entity.GlobalPosition = mousePosNext + DragBeginDisplacement;
                }
            }

            /*
            bool isHovered = HandleBtn.IsHovered();
            if (isHovered != IsCursorDrag)
            {
                IsCursorDrag = isHovered;
                DisplayServer.CursorSetShape(IsCursorDrag ? DisplayServer.CursorShape.Drag : DisplayServer.CursorShape.Arrow);
            }*/
        }
    }

    /*
    public void OnMouseEntered()
    {
        DisplayServer.CursorSetShape(DisplayServer.CursorShape.PointingHand);
    }

    public void OnMouseExited()
    {
        DisplayServer.CursorSetShape(DisplayServer.CursorShape.Arrow);
    }
    */

    protected bool CheckDragging()
    {
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.DragEntity == Entity && mouseDrag.DragHandle == this && IsDragging)
            {
                return true;
            }
        }

        return false;
    }
}


