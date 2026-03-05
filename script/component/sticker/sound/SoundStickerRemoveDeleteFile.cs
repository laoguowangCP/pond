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

    protected OnStickerRemove OnStickerRemove;

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<OnStickerRemove>(out OnStickerRemove);
        OnStickerRemove.StickerRemove += DeleteAudioFile;
    }

    public override bool OnHolderTryRemove()
    {
        OnStickerRemove.StickerRemove -= DeleteAudioFile;
        // Simply return true.
        // Or remove other component you want.
        return base.OnHolderTryRemove();
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