using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class ValidDragAreaVisialize : ComponentResource
{
    public override Type ComponentType => typeof(ValidDragAreaVisialize);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;


    public static readonly NodePath NP_ControlAsValidDragArea = "./UICanvasLayer/BackgroundPanel/GridContainer/PanelMain";
    protected Control ControlAsValidDragArea;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<Control>(NP_ControlAsValidDragArea, out ControlAsValidDragArea);
        Holder.AddTagNoCheck(this, TagEnum.ShowDragArea);
        return true;
    }

    public override void OnEntityReady()
    {
        // Holder.BlockByTag(TagEnum.ShowDragArea);
    }

    
    public override void OnActivated()
    {
        ControlAsValidDragArea.SelfModulate = new Color(1, 1, 1, 1);
    }
    public override void OnDeactivated()
    {
        ControlAsValidDragArea.SelfModulate = new Color(0, 0, 0, 0);
    }
}
