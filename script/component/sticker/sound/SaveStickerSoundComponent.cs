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
    }
}
