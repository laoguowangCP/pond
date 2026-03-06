using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class TestWindowOnFilesDropped : ComponentResource
{
    public override Type ComponentType => typeof(TestWindowOnFilesDropped);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => true;

    public override void OnEntityReady()
    {
        // Hook when window unfocus, exit current drag.
        var window = Holder.GetTree().Root.GetWindow();
        // window.FocusEntered += OnWindowFocusEntered;
        window.FilesDropped += OnWindowFilesDropped;
    }

    public override bool OnHolderTryRemove()
    {
        // Unhook when window unfocus.
        var window = Holder.GetWindow();
        window.FilesDropped -= OnWindowFilesDropped;
        return base.OnHolderTryRemove();
    }
    
    public void OnWindowFilesDropped(string[] files)
    {
        foreach (var file in files)
        {
            
        }
    }
}
