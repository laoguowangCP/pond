using System;
using LGWCP.Util.Collecty;

namespace LGWCP.NiceGD;

public class TagIndexable : IInverseIndexable<TagIndexable>
{
    public int InverseIndex { get; set; } = -1;
    public TagEnum Tag { get; protected set; }
    public IComponent Component { get; protected set; }
    public InverseIndexList<TagIndexable> InverseIndexList { get; set; }

    public TagIndexable(TagEnum tag, IComponent component)
    {
        Tag = tag;
        Component = component;
    }
}

