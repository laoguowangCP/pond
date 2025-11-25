using System;
using System.Collections;
using System.Collections.Generic;
using LGWCP.Util.Collecty;


namespace LGWCP.Nice;

public class Nice
{
    public static readonly Nice I = new();
    public Dictionary<Type, InverseIndexList<RegistIndexable>> ComponentsRegisted;
    public readonly int[] OscillatorsTickGlobal;
    public InverseIndexList<IComponent>[] ComponentsTickGlobal;

    private Nice()
    {
        ComponentsRegisted = new();
        OscillatorsTickGlobal = new int[(int)TickGroupEnum.GlobalGroupCount];
        ComponentsTickGlobal = new InverseIndexList<IComponent>[(int)TickGroupEnum.GlobalGroupCount];
        for (int i = 0; i < ComponentsTickGlobal.Length; ++i)
        {
            OscillatorsTickGlobal[i] = 0;
            ComponentsTickGlobal[i] = new();
        }
    }

    public void AddTickGroupGlobal(IComponent comp)
    {
        int tickGroupIdx = comp.TickGroup - TickGroupEnum.GlobalGroupOffset;
        comp.TickOscillator = OscillatorsTickGlobal[tickGroupIdx];
        ComponentsTickGlobal[tickGroupIdx].TryAdd(comp);
    }

    public void RemoveTickGroupGlobal(IComponent comp)
    {
        int tickGroupIdx = comp.TickGroup - TickGroupEnum.GlobalGroupOffset;
        comp.TickOscillator = -1;
        ComponentsTickGlobal[tickGroupIdx].TryRemove(comp);
    }

    public void Regist(IComponent comp)
    {
        InverseIndexList<RegistIndexable> registIdxabs;
        if (ComponentsRegisted.TryGetValue(comp.ComponentType, out registIdxabs))
        {
            registIdxabs.TryAdd(comp.RegistIdxab);
        }
        else
        {
            registIdxabs = new();
            registIdxabs.TryAdd(comp.RegistIdxab);
            ComponentsRegisted[comp.ComponentType] = registIdxabs;
        }
    }

    public void Unregist(IComponent component)
    {
        if (ComponentsRegisted.TryGetValue(component.ComponentType, out var registIdxabs))
        {
            registIdxabs.TryRemove(component.RegistIdxab);
        }
    }
}
