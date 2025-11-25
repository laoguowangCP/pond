using System;
using System.Collections.Generic;

namespace LGWCP.Util.Pooly;

public class Pooly<T>
    where T : new()
{
    // public static readonly Pooly I = new(Poolicy.I);
    protected readonly Queue<T> Pool = new();
    protected readonly int SizeMax;
    protected int Size;
    protected int ExpandStep;
    protected Func<T> PCreate;
    protected Func<T, bool> PReset;

    public Pooly(int sizeInit, int sizeMax, int expandStep, Poolicy<T> poolicy)
    {
        PCreate = poolicy.Create;
        PReset = poolicy.Reset;
        Size = Math.Min(sizeInit, sizeMax);
        SizeMax = sizeMax;

        for (int i = 0; i < Size; ++i)
        {
            var o = PCreate();
            Pool.Enqueue(o);
        }
    }

    public virtual T Take()
    {
        T o;
        if (Pool.TryDequeue(out o))
        {
            --Size;
        }
        else // Pool is empty
        {
            if (SizeMax > Size)
            {
                Expand();
            }
            o = PCreate();
        }
        return o;
    }

    // Create and enqueue expand step size - 1
    protected virtual void Expand()
    {
        int step = Math.Min(SizeMax - Size, ExpandStep) - 1;
        for (int i = 0; i < step; ++i)
        {
            Pool.Enqueue(PCreate());
        }
    }

    public virtual void GiveBack(T o)
    {
        // Reset and back to pool
        if (Size < SizeMax)
        {
            if (PReset(o))
            {
                Pool.Enqueue(o);
                ++Size;
            }
        }
        else // Pool is filled
        {
            return;
        }
    }
}
