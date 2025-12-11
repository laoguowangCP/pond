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
public partial class DragArea : ComponentResource
{
    public override Type ComponentType => typeof(DragArea);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => true;

    public float WindowMargin = 32;

    public bool CheckAvailableDragArea(Vector2 mousePos)
    {
        Vector2 windowSize = Holder.GetWindow().Size;
        if (mousePos.X < WindowMargin
            || mousePos.X > windowSize.X - WindowMargin
            || mousePos.Y < WindowMargin
            || mousePos.Y > windowSize.Y - WindowMargin)
        {
            return false;
        }
        return true;
    }

    public Vector2 GetRegulatedGlobalPosition(Vector2 pos)
    {
        // Since we wont move or zoom camera
        Vector2 windowSize = Holder.GetWindow().Size;
        // var canvasXform = Holder.GetViewport().GlobalCanvasTransform;
        // GD.Print("GlobalCanvasTransform: ", canvasXform.Origin);
        /*
        Vector2 halfWindowSize = 0.5f * windowSize;
        return pos.Clamp(
            new Vector2(-halfWindowSize.X + WindowMargin, -halfWindowSize.Y + WindowMargin),
            new Vector2(halfWindowSize.X - WindowMargin, halfWindowSize.Y - WindowMargin)
        );
        */
        return pos.Clamp(
            new Vector2(WindowMargin, WindowMargin),
            new Vector2(windowSize.X - WindowMargin, windowSize.Y - WindowMargin)
        );
    }
}
