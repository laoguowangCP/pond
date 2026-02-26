using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BtnAsChangeVolume : ComponentResource
{
    public override Type ComponentType => typeof(BtnAsChangeVolume);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_Button = "./EntityControl/PanelContainer/VBoxContainer/HBoxContainer/Right/VolumeChange";
    protected Button Button;

    protected SoundStickerPlayPause PlayPause;

    public bool IsVolumeAdjust { get; protected set; } = false;
    public float VolumeDb
    {
        get;
        set => field = Math.Clamp(value, -20f, 10f);
    }

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<Button>(NP_Button, out Button);
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<SoundStickerPlayPause>(out PlayPause);
        Button.GuiInput += OnGuiInput;
    }

    public override bool OnHolderTryRemove()
    {
        Button.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    protected void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent
            && mouseEvent.IsReleased())
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                SetVolumeAdjust(IsVolumeAdjust, VolumeDb + 1);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                SetVolumeAdjust(IsVolumeAdjust, VolumeDb - 1);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Middle)
            {
                SetVolumeAdjust(!IsVolumeAdjust, VolumeDb);
            }
        }
    }

    protected void SetVolumeAdjust(bool isVolumeAdjust, float volumeDb)
    {
        Button.SetPressedNoSignal(isVolumeAdjust);
        IsVolumeAdjust = isVolumeAdjust;
        VolumeDb = volumeDb;
        if (isVolumeAdjust)
        {
            Button.TooltipText = "Volume adjust on\n- Mouse middle: toggle volume adjust\n- Mouse left: volume up\n- Mouse right: volume down";
            PlayPause.Player.VolumeDb = VolumeDb;
        }
        else
        {
            Button.TooltipText = "volume adjust off\n- Mouse middle: toggle volume adjust\n- Mouse left: volume up\n- Mouse right: volume down";
            PlayPause.Player.VolumeDb = 0f;
        }
        if (VolumeDb >= 0f)
        {
            Button.Text = $"+{VolumeDb}dB";
            // Button.TooltipText = $"Volume adjust: +{VolumeDbAdjust}dB";
        }
        else
        {
            Button.Text = $"{VolumeDb}dB";
            // Button.TooltipText = $"Volume adjust: {VolumeDbAdjust}dB";
        }
    }

    public void SetVolumeAdjustInitial(bool isVolumeAdjust, float volumeDb)
    {
        SetVolumeAdjust(isVolumeAdjust, volumeDb);
    }
}
