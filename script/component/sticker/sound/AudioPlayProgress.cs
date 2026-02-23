using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class AudioPlayProgress : ComponentResource
{
    public override Type ComponentType => typeof(AudioPlayProgress);
    public override TickGroupEnum TickGroup => TickGroupEnum.Process;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_HSliderAsProgress = "./EntityControl/PanelContainer/VBoxContainer/SliderHBoxContainer/HSlider";
    protected HSlider HSliderAsProgress;

    protected static readonly NodePath NP_LabelAsPlayedTime = "./EntityControl/PanelContainer/VBoxContainer/SliderHBoxContainer/PlayedTimeLabel";
    protected Label LabelAsPlayedTime;

    protected static readonly NodePath NP_LabelAsTotalTime = "./EntityControl/PanelContainer/VBoxContainer/SliderHBoxContainer/TotalTimeLabel";
    protected Label LabelAsTotalTime;

    protected SoundStickerPlayPause PlayPause;


    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<HSlider>(NP_HSliderAsProgress, out HSliderAsProgress);
        Holder.TryGetNodeFromEntity<Label>(NP_LabelAsPlayedTime, out LabelAsPlayedTime);
        Holder.TryGetNodeFromEntity<Label>(NP_LabelAsTotalTime, out LabelAsTotalTime);
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<SoundStickerPlayPause>(out PlayPause);
        Holder.ComponentTickGroupSuspend(this);
    }

    
}
