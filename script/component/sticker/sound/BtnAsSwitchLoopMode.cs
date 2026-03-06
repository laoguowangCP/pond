using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BtnAsSwitchLoopMode : ComponentResource
{
    public override Type ComponentType => typeof(BtnAsSwitchLoopMode);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_Button = "./EntityControl/PanelContainer/VBoxContainer/HBoxContainer/Left/BtnLeft";
    protected Button Button;

    protected SoundStickerPlayPause PlayPause;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<Button>(NP_Button, out Button);
        Button.Pressed += OnSwitchLoopMode;
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<SoundStickerPlayPause>(out PlayPause);
        SetLoopMode(false);
    }

    public override bool OnHolderTryRemove()
    {
        Button.Pressed -= OnSwitchLoopMode;
        return base.OnHolderTryRemove();
    }

    protected void OnSwitchLoopMode()
    {
        SetLoopMode(!PlayPause.IsLoopMode);
    }

    protected void SetLoopMode(bool isLoopMode)
    {
        PlayPause.IsLoopMode = isLoopMode;
        if (isLoopMode)
        {
            // ↔↺↻⇌⇆⇋⇉
            // BtnAsSwitchLoopMode.Text = "∞";
            Button.TooltipText = Tr(Name.Tooltip_LoopModeOn);
        }
        else
        {
            // ⇥
            // BtnAsSwitchLoopMode.Text = "∞";
            Button.TooltipText = Tr(Name.Tooltip_LoopModeOff);
        }
    }

    public void SetLoopModeInitial(bool isLoopMode)
    {
        SetLoopMode(isLoopMode);
        Button.SetPressedNoSignal(isLoopMode);
    }
}
