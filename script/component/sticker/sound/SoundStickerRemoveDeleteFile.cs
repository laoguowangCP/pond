using System;
using System.IO;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SoundStickerRemoveDeleteFile : ComponentResource
{
    public override Type ComponentType => typeof(SoundStickerRemoveDeleteFile);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<OnStickerRemove>(out var onStickerRemove);
        onStickerRemove.StickerRemove += DeleteAudioFile;
    }

    private void DeleteAudioFile()
    {
        Holder.TryGetComponent<SoundStickerLoadAudio>(out var loadAudio);
        if (File.Exists(loadAudio.AudioFile))
        {
            File.Delete(loadAudio.AudioFile);
        }
    }
}