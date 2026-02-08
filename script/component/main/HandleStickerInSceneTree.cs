using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class HandleStickerInSceneTree : ComponentResource
{
    public override Type ComponentType => typeof(HandleStickerInSceneTree);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => true;

    [Export] protected NodePath NP_StickerCanvasLayer = "./MainCanvasLayer";
    protected CanvasLayer StickerCanvasLayer;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<CanvasLayer>(NP_StickerCanvasLayer, out StickerCanvasLayer);
    }

    public void AddSticker(Node2D sticker)
    {
        if (sticker == null)
        {
            return;
        }
        StickerCanvasLayer.AddChild(sticker);
    }

    public void RemoveSticker(Node2D sticker)
    {
        if (sticker == null)
        {
            return;
        }
        sticker.TryGetComponentHolder(out var holder);
        if (holder.TryGetComponent<OnStickerRemove>(out var onStickerRemove))
        {
            onStickerRemove.Trigger();
        }
        holder.HintFreeOnExitTree();
        var parent = sticker.GetParent();
        parent.RemoveChild(sticker);
        sticker.QueueFree();
    }
}
