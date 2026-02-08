using System;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

/// <summary>
/// Provide real size of sticker
/// </summary>
[GlobalClass]
[Tool]
public partial class StickerControlSize : ComponentResource
{
    public override Type ComponentType => typeof(StickerControlSize);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public static readonly NodePath NP_EntityControl = "./EntityControl";
    protected Control EntityControl;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<Control>(NP_EntityControl, out EntityControl);
    }

    public Vector2 Size => EntityControl.Size;
}

