using System;
using System.IO;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SoundStickerPlayPause : ComponentResource
{
    public override Type ComponentType => typeof(SoundStickerPlayPause);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_Player = "./AudioStreamPlayer";
    public AudioStreamPlayer Player;

    protected static readonly NodePath NP_BtnAsPlayPause = "./EntityControl/PanelContainer/VBoxContainer/HBoxContainer/BtnAsPlayPause";
    protected Button BtnAsPlayPause;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<AudioStreamPlayer>(NP_Player, out Player);
        Holder.TryGetNodeFromEntity<Button>(NP_BtnAsPlayPause, out BtnAsPlayPause);

        BtnAsPlayPause.ButtonUp += OnPlayPausePressed;
        Player.Finished += OnPlayerFinished;
        return true;
    }

    public override void OnEntityReady()
    {
        BtnHintPaused();
    }

    protected void OnPlayerFinished()
    {
        Player.StreamPaused = false;
        BtnHintPaused();
        Holder.BlockByTag(TagEnum.SoundStickerAudioPlaying);
    }

    protected void OnPlayPausePressed()
    {
        if (Player.Playing)
        {
            Player.StreamPaused = true;
            BtnHintPaused();
            Holder.BlockByTag(TagEnum.SoundStickerAudioPlaying);
        }
        else
        {
            if (Player.StreamPaused == true)
            {
                Player.StreamPaused = false;
            }
            else
            {
                Player.Play();
            }
            BtnHintPlaying();
            Holder.UnblockByTag(TagEnum.SoundStickerAudioPlaying);
        }
    }

    public override bool OnHolderTryRemove()
    {

        return base.OnHolderTryRemove();
    }

    protected void BtnHintPlaying()
    {
        BtnAsPlayPause.Text = "II";
    }

    protected void BtnHintPaused()
    {
        BtnAsPlayPause.Text = "I>";
    }
}
