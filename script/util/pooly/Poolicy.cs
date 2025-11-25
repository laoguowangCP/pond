using System;
using System.Collections.Generic;

namespace LGWCP.Util.Pooly;

/*
    Poolicy: policy for Pooly.
*/

public class Poolicy<T>
    where T : new()
{
    // public static readonly Poolicy<T> I = new();
    public virtual T Create() => new();

    /// <summary>
    ///  Reset before give back. Do nothing by default.
    /// </summary>
    public virtual bool Reset(T obj) => true;
}
