using Godot;
using Faster.Collections.Pooled;
using System;
using System.Collections.Generic;
using LGWCP.Util.Collecty;

namespace LGWCP.NiceGD;

[GlobalClass]
[Tool]
public abstract partial class ComponentResource : Resource, IComponent
{
    public abstract Type ComponentType { get; }
    public abstract TickGroupEnum TickGroup { get; }

    // Direct inverse indexable replace TickGroupIndexable
    // public TickGroupIndexable TickGroupIdxab { get; set; }
    public int InverseIndex { get; set; } = -1;
    public InverseIndexList<IComponent> InverseIndexList { get; set; }

    public int TickOscillator { get; set; } = -1;
    public abstract bool IsRegist { get; }
    public RegistIndexable RegistIdxab { get; set; }
    public ComponentHolder Holder { get; set; }

    // Tag part
    public PooledList<TagIndexable> TagIdxabs { get; set; } = new();
    // public List<TagIndexable> TagIdxabs { get; set; } = new();

    public ComponentResource()
    {
        ResourceLocalToScene = true;

        // TickGroupIdxab = new(this);
        if (IsRegist)
        {
            RegistIdxab = new(this);
        }
    }

    public int BlockCount { get; set; } = 0;
    public bool IsBlocked { get => BlockCount > 0; }

    // Called when added to holder. True if sure want be added.
    public virtual bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        return true;
    }

    public virtual bool OnHolderTryRemove()
    {
        Holder = null;
        // Simply return true.
        return true;
        // Or remove other component you want.
    }

    public virtual void OnEntityReady()
    {
        // Get ref to other components here.
        // Add tick order here.
    }

    public bool IsActivated { get; set; } = true;
    public virtual void OnActivated() {}
    public virtual void OnDeactivated() {}

    public virtual bool ShouldDeactivate()
    {
        return false;
    }

    public virtual bool ShouldActivate()
    {
        // If not blocked, always activated.
        return true;
    }

    // Try tick after idxab
    public PooledList<TryTickAfterIndexable> TryTickAfterFroms { get; set; }
    // public List<TryTickAfterIndexable> TryTickAfterFroms { get; set; }
    public InverseIndexList<TryTickAfterIndexable> TryTickAfterWaits { get; set; }
    public int TryTickAfterWaitsIdx { get; set; } = 0;

    public virtual void Tick(TickContext ctx) {}
}
