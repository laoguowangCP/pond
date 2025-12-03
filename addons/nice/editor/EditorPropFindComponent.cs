#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;


namespace LGWCP.Nice.Godot;

public partial class EditorPropFindComponent : EditorProperty
{
    private LineEdit _tgtComponentTypeName = new();
    private Button _check = new();
    private ComponentHolder _holder;

    public EditorPropFindComponent()
    {
        var vbox = new VBoxContainer();
        vbox.AddChild(_tgtComponentTypeName);
        vbox.AddChild(_check);
        _check.Text = "🔍Find";
        AddFocusable(_check);
        AddFocusable(_tgtComponentTypeName);
        AddChild(vbox);
        _check.Pressed += OnFindComponentPressed;
    }

    public EditorPropFindComponent(ComponentHolder holder) : this()
    {
        _holder = holder;
    }

    public void OnFindComponentPressed()
    {
        Stack<string> compPath = new();
        List<string> results = new();
        var tgtName = _tgtComponentTypeName.Text;

        compPath.Push("00");
        FindByTypeRecur(tgtName, _holder.Component00, compPath, results);
        compPath.Pop();

        compPath.Push("01");
        FindByTypeRecur(tgtName, _holder.Component01, compPath, results);
        compPath.Pop();

        compPath.Push("02");
        FindByTypeRecur(tgtName, _holder.Component02, compPath, results);
        compPath.Pop();

        compPath.Push("03");
        FindByTypeRecur(tgtName, _holder.Component03, compPath, results);
        compPath.Pop();

        if (tgtName != "")
        {
            GD.PrintRich("Find ", results.Count, " component(s), type name contains [u]", tgtName, "[/u]:");
        }
        else
        {
            GD.PrintRich("Find ", results.Count, " component(s):");
        }
        
        foreach (var result in results)
        {
            GD.PrintRich(result);
        }
        GD.Print();
    }

    protected void FindByTypeRecur(string tgtName, IComponent comp, Stack<string> compPath, List<string> results)
    {
        if (comp is null)
        {
            return;
        }

        // Component sheet, including res ext.
        if (comp is IComponentSheet sheet)
        {
            var comps = sheet.GetSheetComponents();
            string prefixPathlet = ComponentHolderEditorPropUtil.GetPrefixFromSheet(sheet);

            for (int i = 0; i < comps.Length; ++i)
            {
                string pathlet = prefixPathlet;
                pathlet += i.ToString("00");
                compPath.Push(pathlet);
                FindByTypeRecur(tgtName, comps[i], compPath, results);
                compPath.Pop();
            }
        }
        else // normal component
        {
            var compName = comp.ComponentType.ToString().Split('.')[^1];
            if (compName.Contains(tgtName, StringComparison.OrdinalIgnoreCase))
            {
                // Add result
                StringBuilder sbResult = new();
                sbResult.Append("> ").Append(compName).Append(": ");
                sbResult.Append("[u]");
                foreach (var pathlet in compPath.Reverse())
                {
                    sbResult.Append(pathlet);
                    sbResult.Append('/');
                }
                sbResult.Append("[/u]");
                results.Add(sbResult.ToString());
            }
        }
    }
}


#endif
