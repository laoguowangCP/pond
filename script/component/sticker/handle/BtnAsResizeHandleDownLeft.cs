using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BtnAsResizeHandleDownLeft : ComponentResource
{
    public override Type ComponentType => typeof(BtnAsResizeHandleDownLeft);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_HandleBtn = "./EntityControl/HBoxContainer/ResizeHandleBtnDownLeft";
    public static readonly NodePath NP_EntityControl = "./EntityControl";
    protected Button HandleBtn;

    protected Node2D Entity;
    protected Control EntityControl;
    public bool IsDragging { get; protected set; }
    public Vector2 DragBeginPos { get; protected set; } = Vector2.Zero;
    public Vector2 DragBeginEntityPos { get; protected set; } = Vector2.Zero;
    public Vector2 DragBeginEntitySize { get; protected set; } = Vector2.Zero;

    protected EntityControlSizeClamp SizeClamp;
    protected OnStickerSizeChanged OnStickerSizeChanged;

    protected Vector2 PrevMousePos;

    public override void OnEntityReady()
    {
        Holder.TryGetEntity<Node2D>(out Entity);
        Holder.TryGetComponent<EntityControlSizeClamp>(out SizeClamp);
        Holder.TryGetComponent<OnStickerSizeChanged>(out OnStickerSizeChanged);
        Holder.TryGetNodeFromEntity<Control>(NP_EntityControl, out EntityControl);
        Holder.TryGetNodeFromEntity<Button>(NP_HandleBtn, out HandleBtn);
        HandleBtn.FocusMode = Control.FocusModeEnum.None;
        HandleBtn.MouseDefaultCursorShape = Control.CursorShape.PointingHand;

        HandleBtn.ButtonDown += DragBegin;
        HandleBtn.ButtonUp += DragEnd;
        HandleBtn.GuiInput += OnGuiInput;
        DragBeginEntitySize = EntityControl.Size;

        Holder.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited);
        dragEndOnFocusExited.DragEnd += DragEnd;
    }

    public override bool OnHolderTryRemove()
    {
        Holder.TryGetComponent<DragEndOnFocusExited>(out var dragEndOnFocusExited);
        HandleBtn.ButtonDown -= DragBegin;
        HandleBtn.ButtonUp -= DragEnd;
        HandleBtn.GuiInput -= OnGuiInput;
        dragEndOnFocusExited.DragEnd -= DragEnd;
        return base.OnHolderTryRemove();
    }

    public void DragBegin()
    {
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.RequestDragging(Entity, this))
            {
                IsDragging = true;
                // Get mouse pos to entity pos
                DragBeginPos = Entity.GetGlobalMousePosition();
                PrevMousePos = DragBeginPos;
                DragBeginEntitySize = EntityControl.Size;
                DragBeginEntityPos = Entity.GlobalPosition;
                HandleBtn.FocusMode = Control.FocusModeEnum.Click;
                HandleBtn.GrabFocus();
            }
        }
    }

    public void DragEnd()
    {
        if (Nice.I.TryGetRegistedComponentFirst<MouseDrag>(out var mouseDrag))
        {
            if (mouseDrag.UnrequestDragging(Entity))
            {
                IsDragging = false;
                DragBeginPos = Vector2.Zero;
                HandleBtn.FocusMode = Control.FocusModeEnum.None;
                OnStickerSizeChanged.Trigger();
            }
        }
    }

    public void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (CheckDragging())
            {
                // Update entity control size, also gonna change the pos
                Vector2 mousePos = Entity.GetGlobalMousePosition();
                if (Nice.I.TryGetRegistedComponentFirst<DragArea>(out var dragArea))
                {
                    mousePos = dragArea.GetGlobalPositionSoftRegulated(PrevMousePos, mousePos);
                    PrevMousePos = mousePos;
                }
                Vector2 size = new Vector2(
                    (-mousePos + DragBeginPos + DragBeginEntitySize).X,
                    (mousePos - DragBeginPos + DragBeginEntitySize).Y);
                Vector2 nextSize = size.Clamp(SizeClamp.MinSize, SizeClamp.MaxSize);
                EntityControl.Size = nextSize;
                Entity.GlobalPosition = new Vector2(DragBeginEntityPos.X - (nextSize.X - DragBeginEntitySize.X), DragBeginEntityPos.Y);
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
