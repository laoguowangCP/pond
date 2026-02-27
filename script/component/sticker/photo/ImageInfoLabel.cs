using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.WinApiNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class ImageInfoLabel : ComponentResource
{
    public override Type ComponentType => typeof(ImageInfoLabel);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    
    protected static readonly NodePath NP_ImageInfoLabel = "./EntityControl/PanelContainer/VBoxContainer/Label";
    protected Label Label;
    protected List<string> ImageInfo = new();

    protected static readonly StringName SN_CtrlMouseLeft = "ctrl_mouse_left";
    protected static readonly StringName SN_MouseLeft = "mouse_left";
    protected int ShowInfoIdx = 0;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<Label>(NP_ImageInfoLabel, out Label);
        Label.GuiInput += OnGuiInput;
    }

    public override bool OnHolderTryRemove()
    {
        Label.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (Input.IsActionJustPressed(SN_CtrlMouseLeft))
            {
                Holder.TryGetComponent<StickerPhotoLoadImage>(out var loadImage);
                // GD.Print(loadImage.ImageFile);
                DragDropUtil.StartDragDrop(loadImage.ImageFile);
            }
            else if (Input.IsActionJustPressed(SN_MouseLeft))
            {
                ScrollInfo();
            }
        }
    }

    public void UpdateInfo(string file, ImageTexture imageTexture)
    {
        ImageInfo.Clear();
        // StringBuilder sbInfo = new();
        string name = Path.GetFileNameWithoutExtension(file);
        string ext = Path.GetExtension(file);
        Vector2 size = imageTexture.GetSize();
        ImageInfo.Add(name);
        ImageInfo.Add(imageTexture.GetFormat() + " " + size.X + "x" + size.Y + " " + ext);
        Label.Text = ImageInfo[ShowInfoIdx];
    }

    public void ScrollInfo()
    {
        ++ShowInfoIdx;
        if (ShowInfoIdx >= ImageInfo.Count)
        {
            ShowInfoIdx -= ImageInfo.Count;
        }

        Label.Text = ImageInfo[ShowInfoIdx];
    }
}
