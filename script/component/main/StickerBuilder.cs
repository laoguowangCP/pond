using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class StickerBuilder : ComponentResource
{
    public override Type ComponentType => typeof(StickerBuilder);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    [Export] protected NodePath NP_StickerCanvasLayer = "./MainCanvasLayer";
    public CanvasLayer StickerCanvasLayer;

    [Export(PropertyHint.File)] protected string RP_TipSticker = "res://scene/sticker/tip.tscn";
    protected PackedScene TipStickerScene;
    [Export(PropertyHint.File)] protected string RP_PhotoSticker = "res://scene/sticker/photo.tscn";
    protected PackedScene PhotoStickerScene;

    public override void OnEntityReady()
    {
        TipStickerScene = ResourceLoader.Load<PackedScene>(RP_TipSticker);
        PhotoStickerScene = ResourceLoader.Load<PackedScene>(RP_PhotoSticker);
        Holder.TryGetNode<CanvasLayer>(NP_StickerCanvasLayer, out StickerCanvasLayer);
    }

    // Build empty tip sticker by default.
    public Node2D Build()
    {
        var tip = TipStickerScene.Instantiate<Node2D>();
        return tip;
    }
}