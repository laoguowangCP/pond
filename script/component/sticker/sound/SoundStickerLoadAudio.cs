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
    public string AudioFile { get; protected set; }

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<AudioStreamPlayer>(NP_Player, out Player);
    }

    public void LoadFromFile(string file)
    {
        AudioFile = file;
        string ext = Path.GetExtension(file).ToLowerInvariant();
        AudioStream stream;
        if (ext.Equals(".mp3"))
        {
            stream = AudioStreamMP3.LoadFromFile(file);
        }
        else if (ext.Equals(".ogg"))
        {
            stream = AudioStreamOggVorbis.LoadFromFile(file);
        }
        else if (ext.Equals(".wav"))
        {
            stream = AudioStreamOggVorbis.LoadFromFile(file);
        }
        else
        {
            return;
        }
        Player.Stream = stream;
        
        if (Holder.TryGetComponent<AudioInfoLabel>(out var audioInfoLabel))
        {
            audioInfoLabel.UpdateInfo(file, stream);
        }

        if (Holder.TryGetComponent<AudioPlayProgress>(out var audioPlayProgress))
        {
            audioPlayProgress.ResetProgress(stream);
        }
    }
}
