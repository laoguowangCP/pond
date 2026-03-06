#if TOOLS

using Godot;


namespace LGWCP.NiceGD;

[Tool]
public partial class ComponentHolderInspectorMod : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        // We support all objects in this example.
        if (@object is Node node)
        {
            if (node.Name == "ComponentHolder")
            {
                // GD.PrintRich("ComponentHolder focus!");
                return true;
            }
        }
        return false;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type,
        string name, PropertyHint hintType, string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        // hintString is property type
        if (hintString == "ComponentResource" && name == "Component00" && @object is ComponentHolder holder)
        {
            var propFindComp = new EditorPropFindComponent(holder);
            AddPropertyEditor("", propFindComp, label: "==== Find Component ====");
            var propCheckUnique = new EditorPropCheckUnique(holder);
            AddPropertyEditor("", propCheckUnique, label: "==== Check Unique ====");
            var propEmptySlot = new EditorPropEmptySlot(holder);
            AddPropertyEditor("", propEmptySlot, label: "==== Get Empty Slot ====");
        }
        return false;
    }
}


#endif
