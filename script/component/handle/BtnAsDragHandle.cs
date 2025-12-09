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

    public static readonly NodePath NP_HandleBtn = "../EntityControl/DragHandleBtn";
    protected Button HandleBtn;

    protected Node2D Entity;
    public bool IsDragging { get; protected set; }
    public Vector2 DragBeginDisplacement  { get; protected set; } = Vector2.Zero;

    public override void OnEntityReady()
    {
        Holder.TryGetEntity<Node2D>(out Entity);
        HandleBtn = Holder.GetNodeOrNull<Button>(NP_HandleBtn);
        HandleBtn.FocusMode = Control.FocusModeEnum.None;
        HandleBtn.ButtonDown += DragBegin;
        HandleBtn.ButtonUp += DragEnd;
        HandleBtn.GuiInput += OnGuiInput;

        Holder.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited);
        dragEndOnFocusExited.DragEnd += DragEnd;
    }

    public override bool OnHolderTryRemove()
    {
        Holder.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited);
        dragEndOnFocusExited.DragEnd -= DragEnd;
        return base.OnHolderTryRemove();
    }

    public void DragBegin()
    {
        GD.Print(Entity.GetPath(), "DragBegin");
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.RequestDragging(Entity, this))
            {
                IsDragging = true;
                // Get mouse pos to entity pos
                Vector2 mousePos = Entity.GetGlobalMousePosition();
                Vector2 entityPos = Entity.GlobalPosition;
                DragBeginDisplacement = entityPos - mousePos;
            }
        }
    }

    public void DragEnd()
    {
        GD.Print(Entity.GetPath(), "DragEnd");
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.UnrequestDragging(Entity))
            {
                IsDragging = false;
                // Clear displacement
                DragBeginDisplacement = Vector2.Zero;
                // Entity.QueueRedraw();
            }
        }
    }

    public void OnGuiInput(InputEvent @event)
    {
        // GD.Print(Entity.GetPath(), ": OnGuiInput");
        if (CheckDragging())
        {
            // Update entity pos
            Vector2 mousePos = Entity.GetGlobalMousePosition();
            Entity.GlobalPosition = mousePos + DragBeginDisplacement;
            if (!Entity.GetWindow().HasFocus())
            {
                GD.Print("Window unfocused.");
            }
        }
    }

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


