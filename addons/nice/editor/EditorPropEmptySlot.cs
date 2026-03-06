#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace LGWCP.NiceGD;

public partial class EditorPropEmptySlot : EditorProperty
{
    private Button _check = new();
    private ComponentHolder _holder;

    public EditorPropEmptySlot()
    {
        AddChild(_check);
        AddFocusable(_check);
        _check.Text = "📍Get";
        _check.Pressed += OnGetEmptySlotPressed;
    }

    public EditorPropEmptySlot(ComponentHolder holder) : this()
    {
        _holder = holder;
    }

    public void OnGetEmptySlotPressed()
    {
        Stack<string> compPath = new();
        List<string> results = new();

        compPath.Push("00");
        GetEmptySlotRecur(_holder.Component00, compPath, results);
        compPath.Pop();

        compPath.Push("01");
        GetEmptySlotRecur(_holder.Component01, compPath, results);
        compPath.Pop();

        compPath.Push("02");
        GetEmptySlotRecur(_holder.Component02, compPath, results);
        compPath.Pop();

        compPath.Push("03");
        GetEmptySlotRecur(_holder.Component03, compPath, results);
        compPath.Pop();

        if (results.Count == 0)
        {
            GD.PrintRich($"Find {results.Count} empty slot(s).\n");
        }
        else if (results.Count < 3)
        {
            GD.PrintRich($"Find {results.Count} empty slot(s), first at:\n{results[0]}\nConsider add ComponentResExt for sufficient slots.\n");
        }
        else
        {
            GD.PrintRich($"Find {results.Count} empty slot(s), first at:\n{results[0]}\n");
        }
    }

    public void GetEmptySlotRecur(IComponent comp, Stack<string> compPath, List<string> results)
    {
        // Get empty slot and also check if slot count is sufficient.
        // Or it will be painful to add a ext when slot is used up.
        if (comp is null)
        {
            // Add result
            StringBuilder sbResult = new();
            sbResult.Append("> ");
            sbResult.Append("[u]");
            foreach (var pathlet in compPath.Reverse())
            {
                sbResult.Append(pathlet);
                sbResult.Append('/');
            }
            sbResult.Append("[/u]");
            results.Add(sbResult.ToString());

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
                GetEmptySlotRecur(comps[i], compPath, results);
                compPath.Pop();
            }
        }
        // else normal component
    }
}

#endif
