using System;


namespace LGWCP.Util.Collecty;

public interface IInverseIndexable2D<T> : IInverseIndexable<T>
    where T : IInverseIndexable2D<T>
{
    public int InverseIndexX { get; set; }
    public int InverseIndexY { get; set; }
    public InverseIndexList<T> InverseIndexListX { get; set; }
    public InverseIndexList<T> InverseIndexListY { get; set; }
}