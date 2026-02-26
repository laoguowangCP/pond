using System;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.Save;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SaveStickerSoundComponent : ComponentResource
{
    public override Type ComponentType => typeof(SaveStickerSoundComponent);
    public override TickGroupEnum TickGroup => TickGroupEnum.Save;
    public override bool IsRegist => false;

    public override void Tick(TickContext ctx)
    {
        if (ctx.From.Is<SaveRoot>(out var save))
        {
            Holder.TryGetEntity<Node2D>(out var entity);
            Holder.TryGetNodeFromEntity<Control>("./EntityControl", out var control);
            SaveStickerSound soundSave = new(false);
            soundSave.GlobalPosition = entity.Position;
            soundSave.Size = control.Size;
            if (Holder.TryGetComponent<SoundStickerLoadAudio>(out var loadAudio))
            {
                soundSave.AudioFile = loadAudio.AudioFile;
            }
            if (Holder.TryGetComponent<SoundStickerPlayPause>(out var playPause))
            {
                soundSave.TimePlayed = playPause.Player.GetPlaybackPosition();
                soundSave.IsLoopMode = playPause.IsLoopMode;
            }
            if (Holder.TryGetComponent<BtnAsChangeVolume>(out var changeVolume))
            {
                soundSave.IsVolumeAdjust = changeVolume.IsVolumeAdjust;
                soundSave.VolumeDb = changeVolume.VolumeDb;
            }
            save.TryAddListChildrenWithIdx(entity.GetIndex(), soundSave);
        }
    }

    public void Load(SaveStickerSound save)
    {
        Holder.TryGetEntity<Node2D>(out var entity);
        Holder.TryGetNodeFromEntity<Control>("./EntityControl", out var control);
        entity.GlobalPosition = save.GlobalPosition;
        control.Size = save.Size;
        if (Holder.TryGetComponent<SoundStickerLoadAudio>(out var loadAudio))
        {
            loadAudio.LoadFromFile(save.AudioFile);
        }
        if (Holder.TryGetComponent<AudioPlayProgress>(out var audioPlayProgress))
        {
            audioPlayProgress.SetProgressInitial(save.TimePlayed);
        }
        if (Holder.TryGetComponent<BtnAsSwitchLoopMode>(out var loopMode))
        {
            loopMode.SetLoopModeInitial(save.IsLoopMode);
        }
        if (Holder.TryGetComponent<BtnAsChangeVolume>(out var changeVolume))
        {
            changeVolume.SetVolumeAdjustInitial(save.IsVolumeAdjust, save.VolumeDb);
        }
    }
}
