#if TOOLS


namespace LGWCP.Nice.Godot;

public static class ComponentHolderEditorPropUtil
{
    public static string GetPrefixFromSheet(IComponentSheet sheet)
    {
        string prefix;
        if (sheet is ComponentResExt4X)
        {
            prefix = "X";
        }
        else if (sheet is ComponentResExt8X)
        {
            prefix = "X";
        }
        else if (sheet is ComponentResExt16X)
        {
            prefix = "X";
        }
        else if (sheet is ComponentResExt32X)
        {
            prefix = "X";
        }
        else
        {
            prefix = "Sh";
        }

        return prefix;
    }
}

#endif
