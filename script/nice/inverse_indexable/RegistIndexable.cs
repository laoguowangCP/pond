using System;
using LGWCP.Util.Collecty;

namespace LGWCP.NiceGD;

public class RegistIndexable : IInverseIndexable<RegistIndexable>
{
    public int InverseIndex { get; set; } = -1;
    public IComponent Component { get; protected set; }
    public InverseIndexList<RegistIndexable> InverseIndexList { get; set; }
    
    public RegistIndexable(IComponent component)
    {
        Component = component;
    }
}

