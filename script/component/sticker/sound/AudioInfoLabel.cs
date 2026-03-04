using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.WinApiNative;

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

    protected static readonly StringName SN_CtrlMouseLeft = "ctrl_mouse_left";
    protected static readonly StringName SN_MouseLeft = "mouse_left";
    protected int ShowInfoIdx = 0;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<Label>(NP_Label, out Label);
        Label.GuiInput += OnGuiInput;
        // GD.Print(System.Environment.StackTrace);
        return true;
    }

    /*
    public override void OnEntityReady()
    {
        GD.Print("AudioInfoLabel ready.");
        GD.Print(System.Environment.StackTrace);
    }
    */

    public override bool OnHolderTryRemove()
    {
        Label.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonIndex == MouseButton.Left
                    && mouseButton.IsPressed()
                    && mouseButton.ShiftPressed)
                {
                    if (Holder.TryGetComponent<SoundStickerLoadAudio>(out var loadAudio))
                    {
                        DragDropUtil.StartDragDrop(loadAudio.AudioFile);
                    }
                }
                else if (mouseButton.ButtonIndex == MouseButton.Left
                    && mouseButton.IsReleased())
                {
                    if (mouseButton.CtrlPressed)
                    {
                        if (Holder.TryGetComponent<SoundStickerLoadAudio>(out var loadAudio))
                        {
                            if (mouseButton.AltPressed)
                            {
                                OS.ShellShowInFileManager(loadAudio.AudioFile);
                            }
                            else
                            {
                                OS.ShellOpen(loadAudio.AudioFile);
                            }
                        }
                    }
                    else
                    {
                        ScrollInfo();
                    }
                }
            }
        }
    }

    public void UpdateInfo(string file, AudioStream stream)
    {
        ImageInfo.Clear();
        string name = Path.GetFileNameWithoutExtension(file);
        string ext = Path.GetExtension(file);
        TimeSpan length = TimeSpan.FromSeconds(stream.GetLength());
        ImageInfo.Add(name);
        ImageInfo.Add(length.ToString(@"hh\:mm\:ss\.ff") + " " + ext);
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
