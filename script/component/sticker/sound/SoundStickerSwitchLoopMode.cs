using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SoundStickerSwitchLoopMode : ComponentResource
{
    public override Type ComponentType => typeof(SoundStickerSwitchLoopMode);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_BtnAsSwitchLoopMode = "./EntityControl/PanelContainer/VBoxContainer/HBoxContainer/BtnLeft";
    protected Button BtnAsSwitchLoopMode;

    protected SoundStickerPlayPause PlayPause;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<Button>(NP_BtnAsSwitchLoopMode, out BtnAsSwitchLoopMode);
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<SoundStickerPlayPause>(out PlayPause);
        BtnAsSwitchLoopMode.Pressed += OnSwitchLoopMode;
        SetBtnHintLoopMode();
    }

    public override bool OnHolderTryRemove()
    {
        BtnAsSwitchLoopMode.Pressed -= OnSwitchLoopMode;
        return base.OnHolderTryRemove();
    }


    protected void OnSwitchLoopMode()
    {
        PlayPause.SwitchLoopMode();
        SetBtnHintLoopMode();
    }

    protected void SetBtnHintLoopMode()
    {
        if (PlayPause.IsLoopMode)
        {
            // ↔↺↻⇌⇆⇋⇉
            BtnAsSwitchLoopMode.Text = "↻";
        }
        else
        {
            BtnAsSwitchLoopMode.Text = "⇥";
        }
    }
}
