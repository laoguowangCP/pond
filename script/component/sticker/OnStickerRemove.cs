using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class OnStickerRemove : ComponentResource
{
    public override Type ComponentType => typeof(OnStickerRemove);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public delegate void StickerRemoveHandler();
    public event StickerRemoveHandler StickerRemove;

    public void Trigger()
    {
        StickerRemove?.Invoke();
    }
}
