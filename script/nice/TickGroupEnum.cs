namespace LGWCP.NiceGD;

public enum TickGroupEnum: int
{
    Idle = -100, // Component holder is idle, not ticking any group.

    // -------- Node tick --------
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
    SetLocale,

    // -------- Calculated constant group --------
    None, // None ticking group
    GroupCount = None - 1,
    GlobalGroupCount = GroupCount - LocalGroupCount,
    GlobalGroupOffset = LocalGroupCount + 1,
    LowerCheckLocalGroup = LocalGroupCount,
    LowerCheckGlobalGroup = None
}
