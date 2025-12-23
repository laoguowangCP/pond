using System;


namespace LGWCP.NiceGD;

public struct From
{
    public object I;

    public From(object o)
    {
        I = o;
    }

    public readonly bool Is<T>(out T o)
        where T : class
    {
        o = I as T;
        return o != null;
    }
}
