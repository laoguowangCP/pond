using Faster.Collections.Pooled;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LGWCP.Util.Collecty;

/// <summary>
/// Inverse indexed list. Unordered container. O1 in/out, better iterate performance (theoretically).
/// Not that useful for query.
/// </summary>
/// <typeparam name="T"></typeparam>
public class InverseIndexList<T> : IEnumerable<T>, IDisposable
    where T : IInverseIndexable<T>
{
    protected PooledList<T> _values = new();
    public PooledList<T> Values => _values;
    public int Count => _values.Count;
    public T this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    public bool TryAdd(T idxab)
    {
        if (idxab.InverseIndex < 0)
        {
            /*
            if (CurrentCount == _values.Count) // List is full
            {
                _values.Add(idxab);
            }
            else // List has slot on tail
            {
                _values[CurrentCount] = idxab;
            }*/
            idxab.InverseIndex = Count;
            _values.Add(idxab);
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
            var last = _values[Count-1];
            _values[idx] = last;
            last.InverseIndex = idx;
            idxab.InverseIndex = -1;
            _values.RemoveAt(Count - 1);
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
            (_values[yi], _values[xi]) = (_values[xi], _values[yi]);
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

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        _values.Dispose();
        GC.SuppressFinalize(this);
    }

    /*
    ~InverseIndexList()
    {
        // InverseIndexList destruction
    }
    */

    /*
    public struct Enumerator : IEnumerator<T>
    {
        private List<T> _vals;
        private int _cnt;
        private int _pos;

        public Enumerator(InverseIndexList<T> inverseIndexList)
        {
            _vals = inverseIndexList._values;
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

