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
        using (@event)
        {
            
            if (Input.IsActionJustReleased(Name.SN_CtrlScrollUp))
            {
                SetVolumeAdjust(IsVolumeAdjust, VolumeDb + 1);
            }
            else if (Input.IsActionJustReleased(Name.SN_CtrlScrollDown))
            {
                SetVolumeAdjust(IsVolumeAdjust, VolumeDb - 1);
            }
            else if (@event is InputEventMouseButton mouseEvent
                && mouseEvent.IsReleased())
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    SetVolumeAdjust(!IsVolumeAdjust, VolumeDb);
                }
                else if (mouseEvent.ButtonIndex == MouseButton.Middle)
                {
                    SetVolumeAdjust(IsVolumeAdjust, 0);
                }
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
            Button.TooltipText = Tr(Name.Tooltip_LoopModeOn);
            PlayPause.Player.VolumeDb = VolumeDb;
        }
        else
        {
            Button.TooltipText = Tr(Name.ToolTip_VolumeAdjustOff);
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
