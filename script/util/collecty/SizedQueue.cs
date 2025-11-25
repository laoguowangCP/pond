using System;
using System.Collections;
using System.Collections.Generic;

namespace LGWCP.Util.Collecty;

public class SizedQueue<T> : IEnumerable<T>
{
    public const int DefaultSize = 8;
    protected readonly T[] _array;
    protected int _head = 0; // First valid element in the queue. Do not rename (binary serialization)
    protected int _tail = 0; // Last valid element in the queue. Do not rename (binary serialization)
    protected int _count; // Number of elements. Do not rename (binary serialization)

    public int Count => _count;

    public SizedQueue(int size = DefaultSize)
    {
        _array = new T[size];
    }

    public bool Peek(out T val)
    {
        if (Count == 0)
        {
            val = default;
            return false;
        }

        val = _array[_head];
        return true;
    }

    public void Enqueue(T val)
    {
        _array[_tail] = val;
        _tail = (_tail + 1) % _array.Length;
        if (_count == _array.Length)
        {
            _head = _tail;
        }
        else
        {
            ++_count;
        }
    }
    
    // Enqueue with swapped val is return true
    // public bool EnqueueSwap(ref T val) {}

    public bool Dequeue(out T val)
    {
        if (_count == 0)
        {
            val = default;
            return false;
        }

        val = _array[_head];
        _array[_head] = default;
        _head = (_head + 1) % _array.Length;
        --_count;
        return true;
    }

    public virtual void Clear()
    {
        if (_count != 0)
        {
            if (_head < _tail)
                Array.Clear(_array, _head, _count);
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _count = 0;
        }

        _head = 0;
        _tail = 0;
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();


    public struct Enumerator : IEnumerator<T>
    {
        private readonly T[] _array;
        private int _head;
        private int _count;
        private int _pos;

        public Enumerator(SizedQueue<T> sizedQueue)
        {
            _array = sizedQueue._array;
            _head = sizedQueue._head;
            _count = sizedQueue._count;
            _pos = 0;
        }

        public T Current => _array[(_head + _pos) % _array.Length];

        object IEnumerator.Current => Current;

        public void Dispose() {}

        public bool MoveNext()
        {
            // if (_count == 0 || _pos == _count)
            if (_pos == _count)
            {
                return false;
            }
            ++_pos;
            return true;
        }

        public void Reset()
        {
            _pos = 0;
        }
    }
}