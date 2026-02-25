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

    // Used when not played, store seek position for next play.
    protected float NextSeekPosition = 0f;

    public bool IsLoopMode { get; protected set; } = false;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<AudioStreamPlayer>(NP_Player, out Player);
        Holder.TryGetNodeFromEntity<Button>(NP_BtnAsPlayPause, out BtnAsPlayPause);

        BtnAsPlayPause.ButtonUp += OnPlayPausePressed;
        Player.Finished += OnPlayerFinished;
        return true;
    }

    public override bool OnHolderTryRemove()
    {
        BtnAsPlayPause.ButtonUp -= OnPlayPausePressed;
        Player.Finished -= OnPlayerFinished;
        return base.OnHolderTryRemove();
    }

    public override void OnEntityReady()
    {
        BtnHintPaused();
    }

    protected void OnPlayerFinished()
    {
        if (!IsLoopMode)
        {
            Player.StreamPaused = false;
            BtnHintPaused();
            Holder.BlockByTag(TagEnum.SoundStickerAudioPlaying);
        }
        else
        {
            Player.Play();
        }
    }

    protected void OnPlayPausePressed()
    {
        if (Player.Playing)
        {
            Player.StreamPaused = true;
            BtnHintPaused();
            Holder.BlockByTag(TagEnum.SoundStickerAudioPlaying);
        }
        else // Not playing
        {
            if (Player.StreamPaused == true) // Paused
            {
                if (NextSeekPosition >= 0f) // Resume but position changed
                {
                    Player.Play(NextSeekPosition);
                }
                else // Resume no change
                {
                    Player.StreamPaused = false; 
                }
            }
            else // Idle
            {
                if (NextSeekPosition >= 0f) // Play from seek
                {
                    Player.Play(NextSeekPosition);
                }
                else // Play from start
                {
                    Player.Play();
                }
            }
            NextSeekPosition = -1f;
            BtnHintPlaying();
            Holder.UnblockByTag(TagEnum.SoundStickerAudioPlaying);
        }
    }

    protected void BtnHintPlaying()
    {
        BtnAsPlayPause.Text = "II";
    }

    protected void BtnHintPaused()
    {
        BtnAsPlayPause.Text = "I>";
    }

    public void StreamPlayerSeek(float t)
    {
        /*
        1. Playing:
            - If approx to max, stop (or loop mode restart)
            - Else seek
        2. Paused or stop
            - If approx to max, play(0)
            - Else play(t)
        */
        float totalTime = (float)Player.Stream.GetLength();
        bool isApproxMaxHint = MathF.Abs(totalTime - t) / totalTime <= 0.005f;
        if (Player.Playing)
        {
            // If too approx to max, handle as stop
            if (isApproxMaxHint)
            {
                if (!IsLoopMode)
                {
                    Player.Stop();
                    BtnHintPaused();
                    Holder.BlockByTag(TagEnum.SoundStickerAudioPlaying);
                }
                else
                {
                     Player.Play();
                }
            }
            else
            {
                Player.Seek(t);
            }
        }
        else
        {
            // Not playing, store seek position for next play.
            NextSeekPosition = isApproxMaxHint ? 0.0f : t;
        }
    }

    public void SwitchLoopMode()
    {
        IsLoopMode = ! IsLoopMode;
    }
}
