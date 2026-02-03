using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

/// <summary>
/// Available area for handle to drag, or for file to drop.
/// </summary>
[GlobalClass]
[Tool]
public partial class KeepInDragAreaOnWindowSizeChanged : ComponentResource
{
    public override Type ComponentType => typeof(KeepInDragAreaOnWindowSizeChanged);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_EntityControl = "../EntityControl";
    protected Control EntityControl;
    protected Node2D Entity;

    public override void OnEntityReady()
    {
        EntityControl = Holder.GetNodeOrNull<Control>(NP_EntityControl);
        Holder.TryGetEntity<Node2D>(out Entity);
        // Hook when window unfocus, exit current drag.
        var window = Holder.GetTree().Root.GetWindow();
        // window.FocusEntered += OnWindowFocusEntered;
        window.SizeChanged += KeepInDragArea;
    }

    public override bool OnHolderTryRemove()
    {
        // Unhook when window unfocus.
        var window = Holder.GetWindow();
        window.SizeChanged -= KeepInDragArea;
        return base.OnHolderTryRemove();
    }

    public void KeepInDragArea()
    {
        // Get left/right position
        Vector2 topLeft = Entity.GlobalPosition;
        float width = EntityControl.Size.X;
        Vector2 topRight = topLeft + new Vector2(width, 0);

        if (Nice.I.TryGetRegistedComponentFirst<DragArea>(out var dragArea))
        {
            bool checkLeft = dragArea.CheckAvailableDragArea(topLeft);
            bool checkRight = dragArea.CheckAvailableDragArea(topRight);
            if (!checkLeft && !checkRight)
            {
                Vector2 regTopLeft = dragArea.GetGlobalPositionRegulated(topLeft);
                Vector2 regTopRight = dragArea.GetGlobalPositionRegulated(topRight);

                if (MathF.Abs(topLeft.X - regTopLeft.X)
                    > MathF.Abs(topRight.X - regTopRight.X))
                {
                    // Window shrink from left
                    // Saddly it wont happen, whole viewport is moved if window shrinks from left
                    Entity.GlobalPosition = regTopRight - new Vector2(width, 0);
                }
                else
                {
                    // Window shrink from right, or top/bottom shrink
                    Entity.GlobalPosition = regTopLeft;
                }
            }
        }
    }
}