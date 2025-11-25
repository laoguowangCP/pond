using System;


namespace LGWCP.Util.Collecty;

public interface IInverseIndexable<T>
    where T : IInverseIndexable<T>
{
    public int InverseIndex { get; set; }
    public InverseIndexList<T> InverseIndexList { get; set; }
}


