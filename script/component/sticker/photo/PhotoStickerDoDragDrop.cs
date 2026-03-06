using System;
// using System.Windows.Forms;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.WinApiNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class PhotoStickerDoDragDrop : ComponentResource
{
    public override Type ComponentType => typeof(PhotoStickerDoDragDrop);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;
    // protected static readonly NodePath NP_TextureRect = "./EntityControl/PanelContainer/VBoxContainer/TextureRect";
    protected TextureRect TextureRect;
    protected static readonly StringName SN_CtrlMouseLeft = "ctrl_mouse_left";

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<TextureRect>(Name.NP_PhotoStickerTextureRect, out TextureRect);
        TextureRect.GuiInput += OnGuiInput;
    }

    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (Input.IsActionJustPressed(SN_CtrlMouseLeft))
            {
                Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage);
                DragDropUtil.StartDragDrop(loadImage.ImageFile);
            }
            /*
            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
                {
                    Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage);
                    DragDropUtil.StartDragDrop(loadImage.ImageFile);
                }
            }*/
        }
    }
}
