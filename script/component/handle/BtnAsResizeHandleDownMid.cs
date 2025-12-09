using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BtnAsResizeHandleDownMid : ComponentResource
{
    public override Type ComponentType => typeof(BtnAsResizeHandleDownMid);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_HandleBtn = "../EntityControl/HBoxContainer/ResizeHandleBtnDownMid";
    public static readonly NodePath NP_EntityControl = "../EntityControl";
    protected Button HandleBtn;

    protected Node2D Entity;
    protected Control EntityControl;
    public bool IsDragging { get; protected set; }
    public Vector2 DragBeginPos { get; protected set; } = Vector2.Zero;
    public Vector2 DragBeginSize { get; protected set; } = Vector2.Zero;

    protected EntityControlSizeClamp SizeClamp;

    public override void OnEntityReady()
    {
        Holder.TryGetEntity<Node2D>(out Entity);
        Holder.TryGetComponent<EntityControlSizeClamp>(out SizeClamp);
        EntityControl = Holder.GetNodeOrNull<Control>(NP_EntityControl);

        HandleBtn = Holder.GetNodeOrNull<Button>(NP_HandleBtn);
        HandleBtn.FocusMode = Control.FocusModeEnum.None;
        HandleBtn.ButtonDown += DragBegin;
        HandleBtn.ButtonUp += DragEnd;
        HandleBtn.GuiInput += OnGuiInput;
        DragBeginSize = EntityControl.Size;

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
                DragBeginPos = Entity.GetGlobalMousePosition();
                DragBeginSize = EntityControl.Size;
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
                DragBeginPos = Vector2.Zero;
            }
        }
    }

    public void OnGuiInput(InputEvent @event)
    {
        // GD.Print(Entity.GetPath(), ": OnGuiInput");
        if (CheckDragging())
        {
            // Update entity control size y
            Vector2 mousePos = Entity.GetGlobalMousePosition();
            float sizeY = mousePos.Y - DragBeginPos.Y + DragBeginSize.Y;
            EntityControl.Size = new Vector2(DragBeginSize.X, sizeY).Clamp(SizeClamp.MinSize, SizeClamp.MaxSize);
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
