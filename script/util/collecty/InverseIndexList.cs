using Faster.Collections.Pooled;
using System.Collections;
using System.Collections.Generic;

namespace LGWCP.Util.Collecty;

/// <summary>
/// Inverse indexed list. Unordered container. O1 in/out, better iterate performance (theoretically).
/// Not that useful for query.
/// </summary>
/// <typeparam name="T"></typeparam>
public class InverseIndexList<T> : IEnumerable<T>
    where T : IInverseIndexable<T>
{
    public PooledList<T> Values { get; protected set; } = new();
    // public List<T> Values { get; protected set; } = new();
    public int Count => Values.Count;
    public T this[int index]
    {
        get => Values[index];
        set => Values[index] = value;
    }

    public bool TryAdd(T idxab)
    {
        if (idxab.InverseIndex < 0)
        {
            /*
            if (CurrentCount == Values.Count) // List is full
            {
                Values.Add(idxab);
            }
            else // List has slot on tail
            {
                Values[CurrentCount] = idxab;
            }*/
            idxab.InverseIndex = Count;
            Values.Add(idxab);
            idxab.InverseIndexList = this;
            return true;
        }
        else // if (idxab.InverseIndex >= 0)
        {
            return false;
        }
    }

    public bool TryRemove(T idxab)
    {
        int idx = idxab.InverseIndex;
        if (idx < Count && idx >= 0)
        {
            // Swap
            var last = Values[Count-1];
            Values[idx] = last;
            last.InverseIndex = idx;
            idxab.InverseIndex = -1;
            Values.RemoveAt(Count - 1);
            idxab.InverseIndexList = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TrySwap(T x, T y)
    {
        int xi = x.InverseIndex;
        int yi = y.InverseIndex;
        if (xi >= 0 && xi < Count && yi > 0 && yi < Count)
        {
            (Values[yi], Values[xi]) = (Values[xi], Values[yi]);
            x.InverseIndex = yi;
            y.InverseIndex = xi;
            return true;
        }
        else
        {
            return false;
        }
    }

    // public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

    /*
    public struct Enumerator : IEnumerator<T>
    {
        private List<T> _vals;
        private int _cnt;
        private int _pos;

        public Enumerator(InverseIndexList<T> inverseIndexList)
        {
            _vals = inverseIndexList.Values;
            _cnt = inverseIndexList.CurrentCount;
            _pos = -1;
        }

        public T Current => _vals[_pos];

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            ++_pos;
            return _pos < _cnt;
        }

        public void Reset()
        {
            _pos = 0;
        }

        public void Dispose() { }
    }*/
}

