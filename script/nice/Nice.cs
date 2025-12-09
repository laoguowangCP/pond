using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LGWCP.Util.Collecty;


namespace LGWCP.NiceGD;

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

    public bool TryGetRegistedComponents<T>(out InverseIndexList<RegistIndexable> comps)
        where T : IComponent
    {
        if (ComponentsRegisted.TryGetValue(typeof(T), out comps))
        {
            return true;
        }
#if DEBUG
        else
        {
            GD.PushWarning("Nice: component <", typeof(T), "> is expected but not registed.");
        }
#endif
        comps = default;
        return false;
    }

    // Try get registed component but assuming it as global unique component.
    public bool TryGetRegistedComponentFirst<T>(out T comp)
        where T : IComponent
    {
        if (ComponentsRegisted.TryGetValue(typeof(T), out var comps))
        {
            if (comps.Count == 1)
            {
                comp = (T)comps[0].Component;
                return true;
            }
        }
#if DEBUG
        GD.PushWarning("Nice: component <", typeof(T), "> is expected but not registed.");
#endif
        comp = default;
        return false;
    }
}
