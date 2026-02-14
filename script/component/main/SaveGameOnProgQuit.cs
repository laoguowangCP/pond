using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;


/// <summary>
/// Save game on window focus is exited.
/// </summary>
[GlobalClass]
[Tool]
public partial class SaveGameOnProgQuit : ComponentResource
{
    public override Type ComponentType => typeof(SaveGameOnProgQuit);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public override void OnEntityReady()
    {
        // Holder.GetTree().AutoAcceptQuit = false;
        Holder.GetWindow().CloseRequested += OnWindowCloseRequested;
    }

    public override bool OnHolderTryRemove()
    {
        Holder.GetWindow().CloseRequested -= OnWindowCloseRequested;
        return base.OnHolderTryRemove();
    }

    private void OnWindowCloseRequested()
    {
        GD.Print("Window close requested.");
        // Use window close callback instead, or we'll poll notification in node script
        Holder.TryGetComponent<OnSaveGame>(out var onSaveGame);
        onSaveGame.Save();
    }
}
