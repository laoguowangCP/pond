using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class AudioInfoLabel : ComponentResource
{
    public override Type ComponentType => typeof(AudioInfoLabel);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_Label = "./EntityControl/PanelContainer/VBoxContainer/Label";
    protected Label Label;
    protected List<string> ImageInfo = new();

    protected StringName SN_MouseLeft = "mouse_left";
    protected int ShowInfoIdx = 0;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<Label>(NP_Label, out Label);
        Label.GuiInput += OnGuiInput;
    }

    public override bool OnHolderTryRemove()
    {
        Label.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    protected void OnGuiInput(InputEvent @event)
    {
        GD.Print("ImageInfoLabel gui input.");
        if (Input.IsActionJustPressed(SN_MouseLeft))
        {
            ScrollInfo();
        }
    }

    public void UpdateInfo(string file, AudioStream stream)
    {
        ImageInfo.Clear();
        string name = Path.GetFileNameWithoutExtension(file);
        string ext = Path.GetExtension(file);
        TimeSpan length = TimeSpan.FromSeconds(stream.GetLength());
        ImageInfo.Add(name);
        ImageInfo.Add(length.ToString(@"d\.hh\:mm\:ss\.fff") + " " + ext);
        Label.Text = ImageInfo[ShowInfoIdx];
        Label.TooltipText = ImageInfo[ShowInfoIdx];
    }

    public void ScrollInfo()
    {
        ++ShowInfoIdx;
        if (ShowInfoIdx >= ImageInfo.Count)
        {
            ShowInfoIdx -= ImageInfo.Count;
        }

        Label.Text = ImageInfo[ShowInfoIdx];
        Label.TooltipText = ImageInfo[ShowInfoIdx];
    }
}
