#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;


namespace LGWCP.NiceGD;

public partial class EditorPropCheckUnique : EditorProperty
{
    
    private Button _check = new();
    private ComponentHolder _holder;

    public EditorPropCheckUnique()
    {
        AddChild(_check);
        AddFocusable(_check);
        _check.Text = "👁️Check";
        _check.Pressed += OnCheckUniquePressed;
    }

    public EditorPropCheckUnique(ComponentHolder holder) : this()
    {
        _holder = holder;
    }

    public void OnCheckUniquePressed()
    {
        Stack<string> compPath = new();
        List<string> results = new();
        Dictionary<Type, string> kvComps = new();

        compPath.Push("00");
        CheckUniqueRecur(kvComps, _holder.Component00, compPath, results);
        compPath.Pop();

        compPath.Push("01");
        CheckUniqueRecur(kvComps, _holder.Component01, compPath, results);
        compPath.Pop();

        compPath.Push("02");
        CheckUniqueRecur(kvComps, _holder.Component02, compPath, results);
        compPath.Pop();

        compPath.Push("03");
        CheckUniqueRecur(kvComps, _holder.Component03, compPath, results);
        compPath.Pop();

        GD.PrintRich($"Find {results.Count} conflict component(s):");
        foreach (var result in results)
        {
            GD.PrintRich(result);
        }
        GD.Print();
    }
    
    protected void CheckUniqueRecur(Dictionary<Type, string> kvComps, IComponent comp, Stack<string> compPath, List<string> results)
    {
        if (comp is null)
        {
            return;
        }

        // Component sheet, including res ext.
        if (comp is IComponentSheet sheet)
        {
            var comps = sheet.GetSheetComponents();
            string prefix = ComponentHolderEditorPropUtil.GetPrefixFromSheet(sheet);

            for (int i = 0; i < comps.Length; ++i)
            {
                string pathlet = prefix;
                pathlet += i.ToString("00");
                compPath.Push(pathlet);
                CheckUniqueRecur(kvComps, comps[i], compPath, results);
                compPath.Pop();
            }
        }
        else // normal component
        {
            if (kvComps.TryGetValue(comp.ComponentType, out var pathPrev))
            {
                // Conflict, add result
                var compName = comp.ComponentType.ToString().Split('.')[^1];
                StringBuilder sbResult = new();
                sbResult.Append("> ").Append(compName).Append(": ");
                sbResult.Append("[u]");
                foreach (var pathlet in compPath.Reverse())
                {
                    sbResult.Append(pathlet);
                    sbResult.Append('/');
                }
                sbResult.Append("[/u] against [u]");
                sbResult.Append(pathPrev);
                sbResult.Append("[/u]");
                results.Add(sbResult.ToString());
            }
            else // Map this comp
            {
                StringBuilder sbPath = new();
                foreach (var pathlet in compPath.Reverse())
                {
                    sbPath.Append(pathlet);
                    sbPath.Append('/');
                }

                kvComps[comp.ComponentType] = sbPath.ToString();
            }
        }
    }
}

#endif
