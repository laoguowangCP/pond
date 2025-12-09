#if TOOLS
using Godot;
using LGWCP.NiceGD;
using System;

[Tool]
public partial class NicePlugin : EditorPlugin
{
    public static NicePlugin I;
    private ComponentHolderInspectorMod _compHolderMod;

    public NicePlugin()
    {
        I ??= this;
    }

    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        GD.Print("Nice plugin is activated.");
        _compHolderMod = new();
        AddInspectorPlugin(_compHolderMod);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        RemoveInspectorPlugin(_compHolderMod);
    }
}
#endif
