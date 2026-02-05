using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;
using LGWCP.Util.Save;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class StickerBuilder : ComponentResource
{
    public override Type ComponentType => typeof(StickerBuilder);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;


    [Export(PropertyHint.File)] protected string RP_TipSticker = "res://scene/sticker/tip.tscn";
    protected PackedScene TipStickerScene;
    [Export(PropertyHint.File)] protected string RP_PhotoSticker = "res://scene/sticker/photo.tscn";
    protected PackedScene PhotoStickerScene;
    protected HandleStickerInSceneTree HandleSticker;

    public override void OnEntityReady()
    {
        TipStickerScene = ResourceLoader.Load<PackedScene>(RP_TipSticker);
        PhotoStickerScene = ResourceLoader.Load<PackedScene>(RP_PhotoSticker);
        Holder.TryGetComponent<HandleStickerInSceneTree>(out HandleSticker);
    }

    // Build empty tip sticker by default.
    public Node2D Build()
    {
        var tip = TipStickerScene.Instantiate<Node2D>();
        return tip;
    }

    public bool Build(ISaveNode save, out Node2D sticker)
    {
        sticker = default;
        if (save == null)
        {
            return false;
        }

        if (save is SaveStickerTip saveTip)
        {
            sticker = TipStickerScene.Instantiate<Node2D>();
            HandleSticker.AddSticker(sticker);
            if (sticker.TryGetComponent<SaveStickerTipComponent>(out var saveLoad))
            {
                saveLoad.Load(saveTip);
            }
        }
        else if (save is SaveStickerPhoto savePhoto)
        {
            sticker = PhotoStickerScene.Instantiate<Node2D>();
            HandleSticker.AddSticker(sticker);
            if (sticker.TryGetComponent<SaveStickerPhotoComponent>(out var saveLoad))
            {
                saveLoad.Load(savePhoto);
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}