using System;
using LGWCP.Util.Collecty;

namespace LGWCP.NiceGD;

public class TryTickAfterIndexable : IInverseIndexable<TryTickAfterIndexable>
{
    public int InverseIndex { get; set; } = -1;
    public IComponent Waiter { get; protected set; }
    public IComponent Waitee { get; protected set; }
    public InverseIndexList<TryTickAfterIndexable> InverseIndexList { get; set; }

    public TryTickAfterIndexable(IComponent waiter, IComponent waitee)
    {
        Waiter = waiter;
        Waitee = waitee;
    }
}
