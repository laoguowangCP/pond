using System;
using Faster.Collections.Pooled;
using System.Collections.Generic;
using Godot;
using LGWCP.Util.Collecty;


namespace LGWCP.NiceGD;

public class Nice
{
    public const int TickOscillatorSuspend = -1000;
    public const int TickOscillatorDeferedUnsuspend = -999;
    public const int TickOscillatorIdle = -1;

    public static readonly Nice I = new();
    public Dictionary<Type, InverseIndexList<RegistIndexable>> ComponentsRegisted;
    // public Dictionary<Type, InverseIndexList<RegistIndexable>> ComponentsRegisted;
    public readonly int[] OscillatorsTickGlobal;
    public InverseIndexList<IComponent>[] ComponentsTickGlobal;
    protected PooledQueue<(IComponent, IComponent)> TickAfterComponents = new();
    // protected Queue<(IComponent, IComponent)> TickAfterComponents = new();
    public TickGroupEnum TickingTickGroup = TickGroupEnum.Idle;

    private Nice()
    {
        ComponentsRegisted = new();
        OscillatorsTickGlobal = new int[(int)TickGroupEnum.GlobalGroupCount];
        Array.Fill<int>(OscillatorsTickGlobal, 0);
        ComponentsTickGlobal = new InverseIndexList<IComponent>[(int)TickGroupEnum.GlobalGroupCount];
        for (int i = 0; i < ComponentsTickGlobal.Length; ++i)
        {
            ComponentsTickGlobal[i] = new();
        }
    }

    public void AddTickGroupGlobal(IComponent comp, bool isUnsuspend = false)
    {
        if (comp.TickGroup != TickingTickGroup)
        {
            AddTickGroupGlobalMayDeferedPart();
        }
        else
        {
            // Group is ticking, defered add
            comp.TickOscillator = isUnsuspend ? Nice.TickOscillatorDeferedUnsuspend : Nice.TickOscillatorIdle;
            Callable.From(AddTickGroupGlobalMayDeferedPart).CallDeferred();
        }

        void AddTickGroupGlobalMayDeferedPart()
        {
            int tickGroupIdx = comp.TickGroup - TickGroupEnum.GlobalGroupOffset;
            comp.TickOscillator = OscillatorsTickGlobal[tickGroupIdx];
            ComponentsTickGlobal[tickGroupIdx].TryAdd(comp);
        }
    }

    public void RemoveTickGroupGlobal(IComponent comp, bool isSuspend)
    {
        comp.TickOscillator = isSuspend ? Nice.TickOscillatorSuspend : Nice.TickOscillatorIdle;
        if (comp.TickGroup != TickingTickGroup)
        {
            RemoveTickGroupGlobalMayDeferedPart();
        }
        else
        {
            // Group is ticking, defered add
            Callable.From(RemoveTickGroupGlobalMayDeferedPart).CallDeferred();
        }

        void RemoveTickGroupGlobalMayDeferedPart()
        {
            int tickGroupIdx = comp.TickGroup - TickGroupEnum.GlobalGroupOffset;
            ComponentsTickGlobal[tickGroupIdx].TryRemove(comp);
        }
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

    public bool TryGetTickGroup(TickGroupEnum tickGroup, out InverseIndexList<IComponent> comps)
    {
        int tickGroupIdx = tickGroup - TickGroupEnum.GlobalGroupOffset;
        if (tickGroupIdx < ComponentsTickGlobal.Length)
        {
            comps = ComponentsTickGlobal[tickGroupIdx];
            return true;
        }

        comps = null;
        return false;
    }

    public bool TryTick(TickGroupEnum tickGroup, From from)
    {
        int tickGroupIdx = tickGroup - TickGroupEnum.GlobalGroupOffset;
        if (tickGroupIdx < ComponentsTickGlobal.Length)
        {
            DoCheckAndTick(tickGroup, from);
            return true;
        }

        return false;
    }

    // Copy paste from component holder
    protected void DoCheckAndTick(TickGroupEnum tickGroup, From from)
    {
        TickingTickGroup = tickGroup;
        int tickGroupIdx = tickGroup - TickGroupEnum.GlobalGroupOffset;
        int tickOscillator = OscillatorsTickGlobal[tickGroupIdx] == 1 ? 0 : 1;
        OscillatorsTickGlobal[tickGroupIdx] = tickOscillator;
        var comps = ComponentsTickGlobal[tickGroupIdx];

        foreach (var comp in comps)
        {
            if (TryTickAfter(comp, out var waitee, tickOscillator))
            {
                TickAfterComponents.Enqueue((comp, waitee));
            }
            else
            {
                HandleBlockStateTickStateAndDoTick(comp, tickOscillator, from);
            }
        }

        // Handle components trying to wait the other.
        int remainCnt = TickAfterComponents.Count;
        int iterCnt = 0;
        while (remainCnt > 0)
        {
            // Detect cyclic wait
            if (iterCnt == remainCnt) // Cyclic
            {
#if DEBUG
                var (cyclicCompTryWait, cyclicCompBeWaited) = TickAfterComponents.Peek();
                GD.PushWarning(cyclicCompBeWaited.Holder.GetPath(), ": cyclic component try tick after, ", cyclicCompTryWait.ComponentType, " is waiting ", cyclicCompBeWaited.ComponentType);
#endif
                // Tick ignoring order.
                while (TickAfterComponents.TryDequeue(out var compPair))
                {
                    var (comp, _) = compPair;
                    HandleBlockStateTickStateAndDoTick(comp, tickOscillator, from);
                }
                break;
            }
            else // No cyclic
            {
                var (comp, wait) = TickAfterComponents.Dequeue();
                // Swap
                comps.TrySwap(comp, wait);
                if (TryTickAfter(comp, out var waitNext, tickOscillator))
                {
                    // Still wait
                    TickAfterComponents.Enqueue((comp, waitNext));
                    ++iterCnt;
                }
                else
                {
                    HandleBlockStateTickStateAndDoTick(comp, tickOscillator, from);
                    // Reset iter count.
                    iterCnt = 0;
                    --remainCnt;
                }
            }
        }
        TickingTickGroup = TickGroupEnum.Idle;
    }
    
    protected bool TryTickAfter(IComponent comp, out IComponent waitee, int tickOscillator)
    {
        var tryTickAfterWaits = comp.TryTickAfterWaits;
        int tryIdx = comp.TryTickAfterWaitsIdx;
        if (tryTickAfterWaits != null)
        {
            for (; tryIdx < tryTickAfterWaits.Count; ++tryIdx)
            {
                var idxab = tryTickAfterWaits[tryIdx];
                int waitTickOscillator = idxab.Wait.TickOscillator;
                // If not suspended nor idle and not ticked
                if (waitTickOscillator >= 0
                    && waitTickOscillator != tickOscillator)
                {
                    waitee = idxab.Wait;
                    comp.TryTickAfterWaitsIdx = tryIdx; // Give back idx
                    return true;
                }
            }
            tryIdx = 0;
        }
        waitee = null;
        comp.TryTickAfterWaitsIdx = tryIdx; // Give back idx
        return false;
    }

    protected void HandleBlockStateTickStateAndDoTick(IComponent comp, int tickOscillator, From from)
    {
        if (comp.IsActivated)
        {
            if (comp.IsBlocked || comp.ShouldDeactivate())
            {
                comp.IsActivated = false;
                comp.OnDeactivated();
            }
            else
            {
                comp.Tick(new TickContext() { From = from });
            }
        }
        else // not activated
        {
            if (!comp.IsBlocked && comp.ShouldActivate())
            {
                comp.IsActivated = true;
                comp.OnActivated();
                comp.Tick(new TickContext() { From = from });
            }
        }

        comp.TickOscillator = tickOscillator;
    }
}
