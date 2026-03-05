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

    protected float TimeTotal;

    protected bool IsSliderDragging = false;


    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<HSlider>(NP_HSliderAsProgress, out HSliderAsProgress);
        Holder.TryGetNodeFromEntity<Label>(NP_LabelAsPlayedTime, out LabelAsPlayedTime);
        Holder.TryGetNodeFromEntity<Label>(NP_LabelAsTotalTime, out LabelAsTotalTime);

        Holder.TryAddTag(this, TagEnum.SoundStickerAudioPlaying);
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<SoundStickerPlayPause>(out PlayPause);
        
        HSliderAsProgress.DragStarted += OnSliderDragStarted;
        HSliderAsProgress.DragEnded += OnSliderDragEnded;
        HSliderAsProgress.ValueChanged += OnSliderValueChanged;
    }

    public override void OnDeactivated()
    {
        GD.Print("AudioPlayProgress OnDeactivated, block count: ", BlockCount);
        Holder.TickGroupSuspend(this);
    }

    public override void OnActivated()
    {
        GD.Print("AudioPlayProgress OnActivated, block count: ", BlockCount);
        // GD.Print(System.Environment.StackTrace);
        Holder.TickGroupUnsuspend(this);
    }

    protected void OnSliderDragStarted()
    {
        IsSliderDragging = true;
    }

    protected void OnSliderDragEnded(bool valueChanged)
    {
        IsSliderDragging = false;
        if (valueChanged)
        {
            float t = (float)(HSliderAsProgress.Value * TimeTotal / HSliderAsProgress.MaxValue);
            t = Math.Clamp(t, 0.0f, TimeTotal);
            PlayPause.StreamPlayerSeek(t);
            LabelAsPlayedTime.Text = GetLabelTimeFormat(t);
        }
        // GD.Print("Slider value changed: ", valueChanged, ", ", HSliderAsProgress.Value);
    }

    protected void OnSliderValueChanged(double value)
    {
        if (!IsSliderDragging)
        {
            return;
        }
        float t = (float)(value * TimeTotal / HSliderAsProgress.MaxValue);
        LabelAsPlayedTime.Text = GetLabelTimeFormat(t);
    }


    public override void Tick(TickContext ctx)
    {
        // float delta = ctx.AnyDelta;
        // GD.Print("AudioPlayProgress tick.");
        if (!IsSliderDragging && PlayPause.Player.Playing)
        {
            // Slider not dragging, update value.
            float timePlayed = PlayPause.Player.GetPlaybackPosition();
            LabelAsPlayedTime.Text = GetLabelTimeFormat(timePlayed);
            HSliderAsProgress.SetValueNoSignal(timePlayed * 100f / TimeTotal);
        }
        else
        {
            // Slider grabber is dragging
        }
    }

    public void ResetProgress(AudioStream stream)
    {
        LabelAsPlayedTime.Text = GetLabelTimeFormat(0.0f);
        TimeTotal = (float)stream.GetLength();
        LabelAsTotalTime.Text = GetLabelTimeFormat(TimeTotal);
    }

    public void SetProgressInitial(float timePlayed)
    {
        LabelAsPlayedTime.Text = GetLabelTimeFormat(timePlayed);
        HSliderAsProgress.SetValueNoSignal(timePlayed * 100f / TimeTotal);
        PlayPause.StreamPlayerSeek(timePlayed);
    }

    protected static string GetLabelTimeFormat(float second)
    {
        string res;
        var totalTime = TimeSpan.FromSeconds(second);
        if (totalTime.Hours == 0)
        {
            res = totalTime.ToString(@"mm\:ss");
        }
        else
        {
            res = totalTime.ToString(@"hh\:mm\:ss");
        }
        return res;
    }
}
