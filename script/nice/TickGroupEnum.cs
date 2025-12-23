namespace LGWCP.NiceGD;

public enum TickGroupEnum: int
{
    Process = 0,
    PhysicsProcess,
    Input,
    ShortcutInput,
    UnhandledKeyInput,
    UnhandledInput,
    LocalGroupCount,
    // -------- Custom tick --------
    ThermalGamePlay,
    Save,
    None,
    GroupCount = None - 1,
    GlobalGroupCount = GroupCount - LocalGroupCount,
    GlobalGroupOffset = LocalGroupCount + 1,
    LowerCheckLocalGroup = LocalGroupCount,
    LowerCheckGlobalGroup = None
}
