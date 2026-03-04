using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;


/// <summary>
/// Save game on window focus is exited.
/// </summary>
[GlobalClass]
[Tool]
public partial class SaveGameOnWindowFocusExited : ComponentResource
{
    public override Type ComponentType => typeof(SaveGameOnWindowFocusExited);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public bool IsSaveOnWindowFocusExit = true;

    public override void OnEntityReady()
    {
        // Hook when window unfocus, save stickers.
        // var window = Holder.GetWindow();
        // window.FocusExited += OnWindowFocusExited;
    }

    public override bool OnHolderTryRemove()
    {
        // Unhook when window unfocus.
        // var window = Holder.GetWindow();
        // window.FocusExited -= OnWindowFocusExited;
        return base.OnHolderTryRemove();
    }

    public void OnWindowFocusExited()
    {
        if (!IsSaveOnWindowFocusExit)
        {
            return;
        }

        if (Holder.TryGetComponent<OnSaveGame>(out var saver))
        {
            saver.Save();
        }
        // else, focus changed on program kill
    }
}
