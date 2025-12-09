using System;
using LGWCP.Util.Collecty;

namespace LGWCP.NiceGD;

public class TryTickAfterIndexable : IInverseIndexable<TryTickAfterIndexable>
{
    public int InverseIndex { get; set; } = -1;
    public IComponent From { get; protected set; }
    public IComponent Wait { get; protected set; }
    public InverseIndexList<TryTickAfterIndexable> InverseIndexList { get; set; }

    public TryTickAfterIndexable(IComponent from, IComponent wait)
    {
        From = from;
        Wait = wait;
    }
}
