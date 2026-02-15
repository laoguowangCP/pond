namespace LGWCP.NiceGD;

public enum TagEnum: int
{
    // Initial unblocked tags
    FgHover,
     // Any tag greater than this will be blocked after all component OnEntityReady
    InitBlockedTag,
    // Initial blocked tags
    StickerDragging,
    ShowDragArea,
    TagCount
}
