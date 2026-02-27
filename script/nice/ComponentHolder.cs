using Godot;
using System;
using System.Collections.Generic;
using Faster.Collections.Pooled;
using LGWCP.Util.Collecty;
using LGWCP.Extension;
using System.Linq;
using GodotTask;
using System.Text;

namespace LGWCP.NiceGD;

[GlobalClass]
[Tool]
public partial class ComponentHolder : Node, IInverseIndexable<ComponentHolder>, IComponentSheet
{
    public static readonly Action<Node, bool>[] SetNodeActivitys = [
        NodeExtension.StaicSetProcess,
        NodeExtension.StaicSetPhysicsProcess,
        NodeExtension.StaicSetProcessInput,
        NodeExtension.StaicSetProcessShortcutInput,
        NodeExtension.StaicSetProcessUnhandledKeyInput,
        NodeExtension.StaicSetProcessUnhandledInput
    ];

    public static readonly NodePath NodePath = new("ComponentHolder");

    // [Export] protected ComponentResourceSlot ComponentResourceSlot;
    [Export] public ComponentResource Component00;
    [Export] public ComponentResource Component01;
    [Export] public ComponentResource Component02;
    [Export] public ComponentResource Component03;

    public PooledDictionary<Type, IComponent> KVComponents;
    // public readonly Dictionary<Type, IComponent> KVComponents;
    public readonly int[] OscillatorsTickLocal;
    public readonly InverseIndexList<IComponent>[] ComponentsTickLocal;
    public TickGroupEnum TickingTickGroup = TickGroupEnum.Idle;
    public PooledDictionary<TagEnum, InverseIndexList<TagIndexable>> KVTagIdxabs;
    // public readonly Dictionary<TagEnum, InverseIndexList<TagIndexable>> KVTagIdxabs;
    protected bool IsEntityReady = false;
    protected Node Entity;
    protected TickContext TickContext = new();
    protected PooledQueue<(IComponent, IComponent)> TickAfterComponents = new();
    // protected Queue<(IComponent, IComponent)> TickAfterComponents = new();
    protected bool IsFreeOnExitTree = false;

    // Recursive free on exit tree
    protected ComponentHolder DirectAncestorHolder;
    protected InverseIndexList<ComponentHolder> DirectDescendantHolders = new();
    public int InverseIndex { get; set; } = -1;
    public InverseIndexList<ComponentHolder> InverseIndexList { get; set; }

    protected readonly bool[] IsTickOrderStable;
    protected int TickOrderSwapBudget = 8;

    public ComponentHolder()
    {
        KVComponents = new();
        IsTickOrderStable = new bool[(int)TickGroupEnum.LocalGroupCount];
        Array.Fill<bool>(IsTickOrderStable, false);
        OscillatorsTickLocal = new int[(int)TickGroupEnum.LocalGroupCount];
        Array.Fill<int>(OscillatorsTickLocal, 0);
        ComponentsTickLocal = new InverseIndexList<IComponent>[(int)TickGroupEnum.LocalGroupCount];
        for (int i = 0; i < ComponentsTickLocal.Length; ++i)
        {
            ComponentsTickLocal[i] = new();
        }

        TickContext.From = new From(this);
    }

    public override void _EnterTree()
    {
        #if DEBUG
        if (Engine.IsEditorHint())
        {
            return;
        }
        #endif

        Entity = GetParentOrNull<Node>();

        // Find nearest ancestor holder
        Node ancestor = Entity.GetParentOrNull<Node>();
        while (ancestor != null)
        {
            if (ancestor.TryGetComponentHolder(out var ancestorHolder))
            {
                ancestorHolder.AddDirectDescendantHolder(this);
                DirectAncestorHolder = ancestorHolder;
                break;
            }
            ancestor = ancestor.GetParentOrNull<Node>();
        }
    }

    public override void _ExitTree()
    {
        #if DEBUG
        if (Engine.IsEditorHint())
        {
            return;
        }
        #endif

        if (IsFreeOnExitTree)
        {
            Callable.From(this.DeferedFree).CallDeferred();
        }
        DirectAncestorHolder?.RemoveDirectDescendantHolder(this);
    }

    protected void DeferedFree()
    {
        // Remove all components
        var components = KVComponents.Values.ToArray();
        foreach (var comp in components)
        {
            TryRemoveComponent(comp);
        }
    }

    /*
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            // Remove all components
            var components = KVComponents.Values.ToArray();
            foreach (var component in components)
            {
                TryRemoveComponent(component);
            }
        }
    }
    */

    public void HintFreeOnExitTree()
    {
        if (!IsFreeOnExitTree)
        {
            foreach (var holder in DirectDescendantHolders)
            {
                holder.HintFreeOnExitTree();
            }
            IsFreeOnExitTree = true;
        }
    }

    public void CancelFreeOnExitTree()
    {
        if (IsFreeOnExitTree)
        {
            foreach (var holder in DirectDescendantHolders)
            {
                holder.CancelFreeOnExitTree();
            }
            IsFreeOnExitTree = false;
        }
    }

    public void AddDirectDescendantHolder(ComponentHolder holder)
        => DirectDescendantHolders.TryAdd(holder);

    public void RemoveDirectDescendantHolder(ComponentHolder holder)
        => DirectDescendantHolders.TryRemove(holder);

    public override void _Ready()
    {
        #if DEBUG
        if (Engine.IsEditorHint())
        {
            return;
        }
        #endif

        // Deactive node loops by default
        for (int i = 0; i < SetNodeActivitys.Length; ++i)
        {
            SetNodeActivitys[i](this, false);
        }

        if (Component00 != null)
        {
            TryAddComponent(Component00);
        }
        if (Component01 != null)
        {
            TryAddComponent(Component01);
        }
        if (Component02 != null)
        {
            TryAddComponent(Component02);
        }
        if (Component03 != null)
        {
            TryAddComponent(Component03);
        }

        // On entity ready
        OnEntityReadyAsync();
    }

    protected async void OnEntityReadyAsync()
    {
        if (!Entity.IsNodeReady())
        {
            // await ToSignal(Entity, Node.SignalName.Ready);
            await GDTask.FromSignal(Entity, Node.SignalName.Ready);
        }

        IsEntityReady = true;

        var comps = KVComponents.Values.ToArray();
        for (int i = 0; i < comps.Length; ++i)
        {
            comps[i].OnEntityReady();
        }
        
        if (KVTagIdxabs != null)
        {
            foreach ((var tag, var idxabs) in KVTagIdxabs)
            {
                if (tag >= TagEnum.InitBlockedTag)
                {
                    foreach (var idxab in idxabs)
                    {
                        Block(idxab.Component);
                    }
                }
            }
        }
    }

    public bool TryGetEntity<T>(out T entity)
        where T : Node
    {
        entity = Entity as T;
        return entity == null;
    }

    public bool TryAddComponent(IComponent comp)
    {
        if (comp != null && comp.OnHolderTryAdd(this))
        {
            if (KVComponents.TryAdd(comp.ComponentType, comp))
            {
                // Add tick group
                AddTickGroup(comp);
                // Regist Nice
                if (comp.IsRegist)
                {
                    Nice.I.Regist(comp);
                }

                if (IsEntityReady)
                {
                    comp.OnEntityReady();
                }
                // else, wait entity to be ready.
            }
        }

        return false;
    }

    public bool TryRemoveComponent(IComponent comp)
    {
        if (comp != null && comp.OnHolderTryRemove())
        {
            if (KVComponents.Remove(comp.ComponentType))
            {
                // Remove tags
                RemoveAllTags(comp);
                // Clear tick orders
                ClearTickOrders(comp);
                // Remove tick group
                RemoveTickGroup(comp);
                // Regist Nice
                if (comp.IsRegist)
                {
                    Nice.I.Unregist(comp);
                }
            }
        }

        return false;
    }

    public bool TryRemoveComponent<T>()
    {
        if (KVComponents.TryGetValue(typeof(T), out var comp))
        {
            return TryRemoveComponent(comp);
        }

        return false;
    }

    public T GetComponent<T>()
    {
        if (KVComponents.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }
        else
        {
#if DEBUG
            GD.PushWarning(GetPath(), ": component <", typeof(T), "> is expected but not exist.");
#endif
            return default;
        }
    }

    public bool TryGetComponent<T>(out T comp)
        where T : IComponent
    {
        if (KVComponents.TryGetValue(typeof(T), out var value))
        {
            comp = (T)value;
            return true;
        }
/*
#if DEBUG
        else
        {
            GD.PushWarning(GetPath(), ": component <", typeof(T), "> is expected but not exist.");
        }
#endif
*/
        comp = default;
        return false;
    }

    public IComponent[] GetSheetComponents()
    {
        return [
            Component00,
            Component01,
            Component02,
            Component03
        ];
    }

    protected void AddTickGroup(IComponent comp, bool isUnsuspend = false)
    {
        var tickGroup = comp.TickGroup;
        if (tickGroup < TickGroupEnum.LowerCheckLocalGroup)
        {
            // Add local group
            if (TickingTickGroup != tickGroup)
            {
                AddTickGroupMayDeferedPart();
            }
            else
            {
                // Group is ticking, defered add
                comp.TickOscillator = isUnsuspend ? Nice.TickOscillatorDeferedUnsuspend : Nice.TickOscillatorIdle;
                Callable.From(AddTickGroupMayDeferedPart).CallDeferred();
            }
        }
        else if (tickGroup > TickGroupEnum.LowerCheckLocalGroup
            && tickGroup < TickGroupEnum.LowerCheckGlobalGroup)
        {
            // Add global group
            Nice.I.AddTickGroupGlobal(comp, isUnsuspend);
        }

        void AddTickGroupMayDeferedPart()
        {
            comp.TickOscillator = OscillatorsTickLocal[(int)tickGroup];
            ComponentsTickLocal[(int)tickGroup].TryAdd(comp);
            IsTickOrderStable[(int)tickGroup] = false;
            CheckGroupEmptyAndSetActivity(tickGroup);
        }
    }

    protected void RemoveTickGroup(IComponent comp, bool isSuspend = false)
    {
        var tickGroup = comp.TickGroup;
        if (tickGroup < TickGroupEnum.LowerCheckLocalGroup)
        {
            // Remove local group
            comp.TickOscillator = isSuspend ? Nice.TickOscillatorSuspend : Nice.TickOscillatorIdle;
            if (TickingTickGroup != tickGroup)
            {
                RemoveTickGroupMayDeferedPart();
            }
            else
            {
                // Group is ticking, defered remove
                Callable.From(RemoveTickGroupMayDeferedPart).CallDeferred();
            }
        }
        else if (tickGroup > TickGroupEnum.LowerCheckLocalGroup
            && tickGroup < TickGroupEnum.LowerCheckGlobalGroup)
        {
            // Remove global group
            Nice.I.RemoveTickGroupGlobal(comp, isSuspend);
        }

        void RemoveTickGroupMayDeferedPart()
        {
            // GD.Print($"{comp.ComponentType}, {comp.TickOscillator}");
            ComponentsTickLocal[(int)tickGroup].TryRemove(comp);
            CheckGroupEmptyAndSetActivity(tickGroup);
        }
    }

    protected void CheckGroupEmptyAndSetActivity(TickGroupEnum tickGroup)
    {
        int tickGroupIdx = (int)tickGroup;
        bool isNotEmpty = ComponentsTickLocal[tickGroupIdx].Count > 0;
        var setAction = SetNodeActivitys[tickGroupIdx];
        setAction(this, isNotEmpty);
    }

    public bool TryAddTag(IComponent comp, TagEnum tag)
    {
        bool isNewTag = true;
        var tagIdxabs = comp.TagIdxabs;
        if (tagIdxabs != null)
        {
            for (int i = 0; i < tagIdxabs.Count; ++i)
            {
                if (tag == tagIdxabs[i].Tag)
                {
                    isNewTag = false;
                    break;
                }
            }
        }

        if (isNewTag)
        {
            AddTagNoCheck(comp, tag);
        }

        return isNewTag;
    }

    public void AddTagNoCheck(IComponent comp, TagEnum tag)
    {
        // TODO: pool it maybe
        TagIndexable tagIdxab = new(tag, comp);
        if (comp.TagIdxabs == null)
        {
            comp.TagIdxabs = new();
        }
        comp.TagIdxabs.Add(tagIdxab);
        OnComponentAddTag(tagIdxab);
    }

    public bool TryRemoveTag(IComponent comp, TagEnum tag)
    {
        bool isHasTag = false;
        var tagIdxabs = comp.TagIdxabs;
        if (tagIdxabs != null)
        {
            int count = tagIdxabs.Count;
            for (int i = 0; i < count; ++i)
            {
                if (tag == tagIdxabs[i].Tag)
                {
                    isHasTag = true;
                    OnComponentRemoveTag(tagIdxabs[i]);
                    tagIdxabs[i] = tagIdxabs[count - 1];
                    tagIdxabs.RemoveAt(count - 1);
                    break;
                }
            }
        }

        return isHasTag;
    }

    public void RemoveAllTags(IComponent comp)
    {
        var tagIdxabs = comp.TagIdxabs;
        if (tagIdxabs != null)
        {
            for (int i = 0; i < tagIdxabs.Count; ++i)
            {
                OnComponentRemoveTag(tagIdxabs[i]);
            }
        }
        tagIdxabs.Clear();
    }

    // Map tag indexable: called whenever component has a tag added, during start up or running.
    public void OnComponentAddTag(TagIndexable tagIdxab)
    {
        if (KVTagIdxabs == null)
        {
            KVTagIdxabs = new();
        }

        InverseIndexList<TagIndexable> tagIdxabs;
        if (KVTagIdxabs.TryGetValue(tagIdxab.Tag, out tagIdxabs))
        {
            tagIdxabs.TryAdd(tagIdxab);
        }
        else
        {
            tagIdxabs = new();
            tagIdxabs.TryAdd(tagIdxab);
            KVTagIdxabs[tagIdxab.Tag] = tagIdxabs;
        }
    }

    public void OnComponentRemoveTag(TagIndexable tagIdxab)
    {
        if (KVTagIdxabs.TryGetValue(tagIdxab.Tag, out var tagIdxabs))
        {
            tagIdxabs.TryRemove(tagIdxab);
        }
    }

    public void Block(IComponent comp)
    {
        // GD.Print("Block ", comp.ComponentType);
        ++comp.BlockCount;
        // Handle none tick component
        if (comp.BlockCount == 1
            && (comp.TickGroup == TickGroupEnum.None || comp.TickOscillator < Nice.TickOscillatorIdle))
        {
            // comp.IsBlocked = true;
            comp.IsActivated = false;
            comp.OnDeactivated();
        }
    }

    public void Unblock(IComponent comp)
    {
        // GD.Print("Try unblock ", comp.ComponentType);
        --comp.BlockCount;
        // GD.Print("- ", comp.BlockCount);
        // GD.Print("- ", comp.TickGroup);
        // GD.Print("- ", comp.TickOscillator);
        if (comp.BlockCount == 0
            && (comp.TickGroup == TickGroupEnum.None || comp.TickOscillator < Nice.TickOscillatorIdle))
        {
            // comp.IsBlocked = false;
            comp.IsActivated = true;
            comp.OnActivated();
        }
#if DEBUG
        if (comp.BlockCount < 0)
        {
            GD.PushWarning(GetPath(), ": component [", comp.ComponentType, "] is over unblocked.");
        }
#endif
    }

    public void BlockByTag(TagEnum tag)
    {
        if (KVTagIdxabs.TryGetValue(tag, out var tagIdxabs))
        {
            foreach (var tagIdxab in tagIdxabs)
            {
                Block(tagIdxab.Component);
            }
        }
    }

    public void BlockByTagExcept(TagEnum tag, IComponent except)
    {
        if (KVTagIdxabs.TryGetValue(tag, out var tagIdxabs))
        {
            foreach (var tagIdxab in tagIdxabs)
            {
                var comp = tagIdxab.Component;
                if (comp != except)
                {
                    Block(comp);
                }
            }
        }
    }

    public void UnblockByTag(TagEnum tag)
    {
        if (KVTagIdxabs.TryGetValue(tag, out var tagIdxabs))
        {
            foreach (var tagIdxab in tagIdxabs)
            {
                Unblock(tagIdxab.Component);
            }
        }
    }

    public void UnblockByTagExcept(TagEnum tag, IComponent except)
    {
        if (KVTagIdxabs.TryGetValue(tag, out var tagIdxabs))
        {
            foreach (var tagIdxab in tagIdxabs)
            {
                var comp = tagIdxab.Component;
                if (comp != except)
                {
                    Unblock(comp);
                }
            }
        }
    }

    /// <summary>
    /// Comp should tick after T in this holder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comp"></param>
    public void ShouldTickAfter<T>(IComponent comp)
        where T : IComponent
    {
        if (TryGetComponent<T>(out var wait))
        {
            ShouldTickOrder(from: comp, wait);
        }
    }

    /// <summary>
    /// Comp should tick before T in this holder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comp"></param>
    public void ShouldTickBefore<T>(IComponent comp)
        where T : IComponent
    {
        if (TryGetComponent<T>(out var from))
        {
            ShouldTickOrder(from, wait: comp);
        }
    }

    public void ShouldTickOrder(IComponent from, IComponent wait)
    {
        if (from.TickGroup != wait.TickGroup)
        {
#if DEBUG
            GD.PushWarning(GetPath(), ": should tick order ", from.ComponentType, " after ", wait.ComponentType, " not in same tick group.");
#endif
            return;
        }

        TryTickAfterIndexable idxab = new(from, wait);
        from.TryTickAfterWaits ??= new();
        from.TryTickAfterWaits.TryAdd(idxab);

        wait.TryTickAfterFroms ??= new();
        wait.TryTickAfterFroms.Add(idxab);
    }

    protected void ClearTickOrders(IComponent comp)
    {
        // Don't wait comp anymore
        var tryTickAfterFroms = comp.TryTickAfterFroms;
        if (tryTickAfterFroms != null)
        {
            for (int i = 0; i < tryTickAfterFroms.Count; ++i)
            {
                var idxab = tryTickAfterFroms[i];
                idxab.InverseIndexList.TryRemove(idxab);
            }
            tryTickAfterFroms.Clear();
        }
    }

    public override void _Process(double delta)
    {
        TickContext.AnyDelta = (float)delta;
        DoCheckAndTick(TickGroupEnum.Process);
    }

    public override void _PhysicsProcess(double delta)
    {
        TickContext.AnyDelta = (float)delta;
        DoCheckAndTick(TickGroupEnum.PhysicsProcess);
    }

    public override void _Input(InputEvent input)
    {
        using (input)
        {
            TickContext.AnyInput = input;
            DoCheckAndTick(TickGroupEnum.Input);
        }
    }

    public override void _ShortcutInput(InputEvent input)
    {
        using (input)
        {
            TickContext.AnyInput = input;
            DoCheckAndTick(TickGroupEnum.ShortcutInput);
        }
    }

    public override void _UnhandledKeyInput(InputEvent input)
    {
        using (input)
        {
            TickContext.AnyInput = input;
            DoCheckAndTick(TickGroupEnum.UnhandledKeyInput);
        }
    }

    public override void _UnhandledInput(InputEvent input)
    {
        using (input)
        {
            TickContext.AnyInput = input;
            DoCheckAndTick(TickGroupEnum.UnhandledInput);
        }
    }

    protected void DoCheckAndTick(TickGroupEnum tickGroup)
    {
        /*
        if (TickingTickGroup != TickGroupEnum.Idle)
        {
            GD.Print("TickingTickGroup is not cleared: ", TickingTickGroup);
        }
        */
        TickingTickGroup = tickGroup;
        int tickGroupIdx = (int)tickGroup;
        int tickOscillator = OscillatorsTickLocal[tickGroupIdx] == 1 ? 0 : 1;
        OscillatorsTickLocal[tickGroupIdx] = tickOscillator;
        var comps = ComponentsTickLocal[tickGroupIdx];

        if (IsTickOrderStable[tickGroupIdx])
        {
            foreach (var comp in comps)
            {
                HandleBlockStateTickStateAndDoTick(comp, tickOscillator);
            }
        }
        else // Tick order not stable, should test tick order
        {
            foreach (var comp in comps)
            {
                if (TryTickAfter(comp, out var wait, tickOscillator))
                {
                    TickAfterComponents.Enqueue((comp, wait));
                }
                else
                {
                    HandleBlockStateTickStateAndDoTick(comp, tickOscillator);
                }
            }
        }

        // Handle components trying to wait the other.
        int remainCnt = TickAfterComponents.Count;
        if (remainCnt == 0)
        {
            IsTickOrderStable[tickGroupIdx] = true;
        }
        else // Handle tick order
        {
            int iterCnt = 0;
            int swapBudget = TickOrderSwapBudget;
            while (remainCnt > 0)
            {
                // Detect cyclic wait
                if (iterCnt != remainCnt) // No cyclic
                {
                    var (comp, wait) = TickAfterComponents.Dequeue();
                    // Swap
                    if (swapBudget > 0)
                    {
                        comps.TrySwap(comp, wait);
                        --swapBudget;
                    }
                    if (TryTickAfter(comp, out var waitNext, tickOscillator))
                    {
                        // Still wait
                        TickAfterComponents.Enqueue((comp, waitNext));
                        ++iterCnt;
                    }
                    else
                    {
                        HandleBlockStateTickStateAndDoTick(comp, tickOscillator);
                        // Reset iter count.
                        iterCnt = 0;
                        --remainCnt;
                    }
                }
                else // Cyclic
                {
    #if DEBUG
                    StringBuilder cyclicWarningBuilder = new();
                    cyclicWarningBuilder
                        .Append(this.GetPath())
                        .Append(": cyclic component try tick after:\n");
                    foreach (var (cyclicFrom, cyclicWait) in TickAfterComponents)
                    {
                        cyclicWarningBuilder
                            .Append("- [{cyclicFrom.ComponentType}] waits [{cyclicWait.ComponentType}]");
                    }
                    GD.PushWarning(cyclicWarningBuilder.ToString());
    #endif
                    // Tick ignoring order.
                    while (TickAfterComponents.TryDequeue(out var compPair))
                    {
                        var (comp, _) = compPair;
                        HandleBlockStateTickStateAndDoTick(comp, tickOscillator);
                    }
                    break;
                }
            }
        }
        TickingTickGroup = TickGroupEnum.Idle;
    }
    
    protected bool TryTickAfter(IComponent comp, out IComponent wait, int tickOscillator)
    {
        var tryTickAfterWaits = comp.TryTickAfterWaits;
        int tryIdx = comp.TryTickAfterWaitsIdx;
        if (tryTickAfterWaits != null)
        {
            for (; tryIdx < tryTickAfterWaits.Count; ++tryIdx)
            {
                var idxab = tryTickAfterWaits[tryIdx];
                // if (!idxab.Wait.IsTicked)
                int waitTickOscillator = idxab.Wait.TickOscillator;
                // If not suspended nor idle and not ticked
                if (waitTickOscillator >= 0
                    && waitTickOscillator != tickOscillator)
                {
                    wait = idxab.Wait;
                    comp.TryTickAfterWaitsIdx = tryIdx; // Give back idx
                    return true;
                }
            }
            tryIdx = 0;
        }
        wait = null;
        comp.TryTickAfterWaitsIdx = tryIdx; // Give back idx
        return false;
    }

    protected void HandleBlockStateTickStateAndDoTick(IComponent comp, int tickOscillator)
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
                comp.Tick(TickContext);
            }
        }
        else // not activated
        {
            if (!comp.IsBlocked && comp.ShouldActivate())
            {
                comp.IsActivated = true;
                comp.OnActivated();
                comp.Tick(TickContext);
            }
        }

        if (comp.TickOscillator >= 0)
        {
            comp.TickOscillator = tickOscillator;
        }
        // else, component may be suspended during tick, leave it.
    }

    public bool TryGetNodeFromEntity<T>(NodePath pathFromEntity, out T node)
        where T : Node
    {
        node = Entity?.GetNodeOrNull<T>(pathFromEntity);
        return node == null;
    }

    public bool TryGetNode<T>(NodePath pathFromHolder, out T node)
        where T : Node
    {
        node = GetNodeOrNull<T>(pathFromHolder);
        return node == null;
    }

    /// <summary>
    /// Suspend component, move out tick group.\n
    /// OnActivated/OnDeactivated callback will be triggered once unblocked/blocked, as tick group none do.
    /// </summary>
    public void TickGroupSuspend(IComponent comp)
    {
        if (comp.Holder == this
            && comp.TickGroup != TickGroupEnum.None
            && comp.TickOscillator != Nice.TickOscillatorSuspend)
        {
            RemoveTickGroup(comp, isSuspend: true);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Unsuspend component, back to tick group.
    /// </summary>
    public void TickGroupUnsuspend(IComponent comp)
    {
        GD.Print("TickGroupUnsuspend");
        if (comp.Holder == this
            && comp.TickGroup != TickGroupEnum.None
            && comp.TickOscillator == Nice.TickOscillatorSuspend)
        {
            AddTickGroup(comp, isUnsuspend: true);
        }
        else
        {
            return;
        }
    }
}
