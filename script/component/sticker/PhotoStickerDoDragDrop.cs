using System;
using System.Windows.Forms;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.OleNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class PhotoStickerDoDragDrop : ComponentResource
{
    public override Type ComponentType => typeof(PhotoStickerDoDragDrop);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;



}
