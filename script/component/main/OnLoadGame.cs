using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Godot;
using GodotTask;
using LGWCP.NiceGD;
using LGWCP.Util.Save;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class OnLoadGame : ComponentResource
{
    public override Type ComponentType => typeof(OnLoadGame);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<OnSaveGame>(out var onSaveGame);
        Callable.From(onSaveGame.Load).CallDeferred();
    }
}
