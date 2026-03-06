using System;
using System.IO;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SoundStickerLoadAudio : ComponentResource
{
    public override Type ComponentType => typeof(SoundStickerLoadAudio);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_Player = "./AudioStreamPlayer";
    protected AudioStreamPlayer Player;
    protected AudioStream StreamRes;
    public string AudioFile { get; protected set; }

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<AudioStreamPlayer>(NP_Player, out Player);
    }

    public override bool OnHolderTryRemove()
    {
        Player.Stream = null;
        StreamRes?.Dispose();
        return base.OnHolderTryRemove();
    }

    public void LoadFromFile(string file)
    {
        AudioFile = file;
        if (!File.Exists(AudioFile))
        {
            return;
        }
        string ext = Path.GetExtension(file).ToLowerInvariant();
        if (ext.Equals(".mp3"))
        {
            StreamRes = AudioStreamMP3.LoadFromFile(file);
        }
        else if (ext.Equals(".ogg"))
        {
            StreamRes = AudioStreamOggVorbis.LoadFromFile(file);
        }
        else if (ext.Equals(".wav"))
        {
            StreamRes = AudioStreamOggVorbis.LoadFromFile(file);
        }
        else
        {
            return;
        }
        Player.Stream = StreamRes;
        
        if (Holder.TryGetComponent<AudioInfoLabel>(out var audioInfoLabel))
        {
            audioInfoLabel.UpdateInfo(file, StreamRes);
        }

        if (Holder.TryGetComponent<AudioPlayProgress>(out var audioPlayProgress))
        {
            audioPlayProgress.ResetProgress(StreamRes);
        }
    }
}
