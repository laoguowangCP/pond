using System;
using System.Windows.Forms;
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
    protected static readonly NodePath NP_TextureRect = "./EntityControl/PanelContainer/TextureRect";
    protected TextureRect TextureRect;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<TextureRect>(NP_TextureRect, out TextureRect);
        TextureRect.GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        // GD.Print(@event.AsText());
        if (Input.IsActionJustPressed("ctrl_mouse_left"))
        {
            Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage);
            GD.Print(loadImage.ImageFile);
            DragDropUtil.StartDragDrop(loadImage.ImageFile);
        }
        /*
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage);
                GD.Print(loadImage.ImageFile);
                DragDropUtil.StartDragDrop(loadImage.ImageFile);
            }
        }*/
    }
}
