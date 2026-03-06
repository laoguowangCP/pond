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

    public float WindowMarginUp = 5;
    public float WindowMarginLR = 32;
    public float WindowMarginDown = 5;

    protected NodePath NP_MainPanel = "./UICanvasLayer/BackgroundPanel/GridContainer/PanelMain";
    protected Panel MainPanel;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<Panel>(NP_MainPanel, out MainPanel);
    }

    public bool CheckAvailableDragArea(Vector2 mousePos)
    {
        /*
        Vector2 windowSize = Holder.GetWindow().Size;
        if (mousePos.X < WindowMarginLR
            || mousePos.X > windowSize.X - WindowMarginLR
            || mousePos.Y < WindowMarginUp
            || mousePos.Y > windowSize.Y - WindowMarginDown)
        {
            return false;
        }
        return true;
        */

        bool isShow;
        var ctrlPos = MainPanel.GlobalPosition;
        var ctrlSize = MainPanel.GetRect().Size;

        if (mousePos.X >= ctrlPos.X
            && mousePos.X <= ctrlPos.X + ctrlSize.X
            && mousePos.Y >= ctrlPos.Y
            && mousePos.Y <= ctrlPos.Y + ctrlSize.Y)
        {
            // Inside delete area
            isShow = true;
        }
        else
        {
            isShow = false;
        }
        return isShow;
    }

    // Return position regulated by area
    public Vector2 GetGlobalPositionRegulated(Vector2 pos)
    {
        // Since we wont move or zoom camera
        
        // var canvasXform = Holder.GetViewport().GlobalCanvasTransform;
        /*
        Vector2 halfWindowSize = 0.5f * windowSize;
        return pos.Clamp(
            new Vector2(-halfWindowSize.X + WindowMargin, -halfWindowSize.Y + WindowMargin),
            new Vector2(halfWindowSize.X - WindowMargin, halfWindowSize.Y - WindowMargin)
        );
        */

        /*
        Vector2 windowSize = Holder.GetWindow().Size;
        float maxX = windowSize.X - WindowMarginLR > WindowMarginLR ? windowSize.X - WindowMarginLR : WindowMarginLR;
        float maxY = windowSize.Y - WindowMarginDown > WindowMarginUp ? windowSize.Y - WindowMarginDown : WindowMarginUp;
        return pos.Clamp(
            new Vector2(WindowMarginLR, WindowMarginUp),
            new Vector2(maxX, maxY)
        );
        */
        var ctrlPos = MainPanel.GlobalPosition;
        var ctrlSize = MainPanel.GetRect().Size;
        return pos.Clamp(
            ctrlPos,
            ctrlPos + ctrlSize);
    }

    public Vector2 GetGlobalPositionSoftRegulated(Vector2 posPrev, Vector2 posNextRaw)
    {
        /*
        Vector2 windowSize = Holder.GetWindow().Size;
        float maxX = windowSize.X - WindowMarginLR > WindowMarginLR ? windowSize.X - WindowMarginLR : WindowMarginLR;
        float maxY = windowSize.Y - WindowMarginDown > WindowMarginUp ? windowSize.Y - WindowMarginDown : WindowMarginUp;

        var posNext = posNextRaw;
        if (posPrev.X < WindowMarginLR)
        {
            posNext.X = Mathf.Max(posPrev.X, posNextRaw.X);
        }
        else if (posPrev.X > maxX)
        {
            posNext.X = Mathf.Min(posPrev.X, posNextRaw.X);
        }
        
        if (posPrev.Y < WindowMarginUp)
        {
            posNext.Y = Mathf.Max(posPrev.Y, posNextRaw.Y);
        }
        else if (posPrev.Y > maxY)
        {
            posNext.Y = Mathf.Min(posPrev.Y, posNextRaw.Y);
        }

        return posNext;
        */
        var ctrlPos = MainPanel.GlobalPosition;
        var ctrlSize = MainPanel.GetRect().Size;
        var posNext = posNextRaw;
        if (posPrev.X < ctrlPos.X)
        {
            posNext.X = Mathf.Max(posPrev.X, posNextRaw.X);
        }
        else if (posPrev.X > ctrlPos.X + ctrlSize.X)
        {
            posNext.X = Mathf.Min(posPrev.X, posNextRaw.X);
        }
        
        if (posPrev.Y < ctrlPos.Y)
        {
            posNext.Y = Mathf.Max(posPrev.Y, posNextRaw.Y);
        }
        else if (posPrev.Y > ctrlPos.Y + ctrlSize.Y)
        {
            posNext.Y = Mathf.Min(posPrev.Y, posNextRaw.Y);
        }

        return posNext;
    }
}
