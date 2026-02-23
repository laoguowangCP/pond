using System;
using System.Collections.Generic;
using Faster.Collections.Pooled;
using LGWCP.Util.Collecty;

namespace LGWCP.NiceGD;

public interface IComponent : IInverseIndexable<IComponent>
{
    public Type ComponentType { get; }
    public TickGroupEnum TickGroup { get; }

    // Direct inverse indexable replace TickGroupIndexable
    // public TickGroupIndexable TickGroupIdxab { get; set; }

    public int TickOscillator { get; set; }
    public bool IsRegist { get; }
    public RegistIndexable RegistIdxab { get; set; }

    // Tag part
    // protected List<TagIndexable> TagIdxabs;
    public PooledList<TagIndexable> TagIdxabs { get; set; }
    // public List<TagIndexable> TagIdxabs { get; set; }
    public int BlockCount { get; set; }
    public bool IsBlocked { get; }
    public ComponentHolder Holder { get; set; }

    // Lifetime part

    /// <summary>
    /// Called when holder try to add this component, true if should be added.
    /// </summary>
    public bool OnHolderTryAdd(ComponentHolder holder);
    /// <summary>
    /// Called when holder try to remove this component, true if should be removed.
    /// </summary>
    public bool OnHolderTryRemove();
    /// <summary>
    /// Called when entity is ready. Add tags here.\n
    /// For fixed components, get other components here.\n
    /// For components attached during runtime, only fixed components can be fetched.
    /// </summary>
    public void OnEntityReady();
    
    public bool IsActivated { get; set; }
    public bool ShouldDeactivate();
    public bool ShouldActivate();
    public void OnActivated();
    public void OnDeactivated();

    // Try tick after idxab
    public PooledList<TryTickAfterIndexable> TryTickAfterFroms { get; set; }
    // public List<TryTickAfterIndexable> TryTickAfterFroms { get; set; }
    public InverseIndexList<TryTickAfterIndexable> TryTickAfterWaits { get; set; }
    // For component holder remember where to continue try tick after.
    public int TryTickAfterWaitsIdx { get; set; }

    public void Tick(TickContext ctx);
}
