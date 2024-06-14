using System;

public class Heap<T> where T : IComparable<T>
{
    #region Variables
    private int _count = 0;
    private T[] _heap = null;
    #endregion

    #region Properties
    public int Count { get { return _count; } }
    public bool IsEmpty { get { return _count == 0; } }
    #endregion

    #region Methods - Constructor
    public Heap(int minSize = 20)
    {
        _heap = new T[minSize];
        _count = 0;
    }
    #endregion

    #region Methods - Private
    private void SiftUp()
    {
        int child = _count - 1;
        int parent = (child - 1) / 2;

        while (child > 0 && Compare(child, parent) < 0)
        {
            Swap(child, parent);
            child = parent;
            parent = (child - 1) / 2;
        }
    }

    private void SiftDown(int curr, int end)
    {
        int left = (curr * 2) + 1;
        while (left <= end)
        {
            int right = left + 1;
            if (right > end)
            {
                right = -1;
            }

            int swap = left;
            if (right != -1 && Compare(left, right) > 0)
            {
                swap = right;
            }

            if (Compare(swap, curr) < 0)
            {
                Swap(swap, curr);
                curr = swap;
                left = (curr * 2) + 1;
            }
            else
            {
                return;
            }
        }
    }

    private int Compare(int a, int b)
    {
        return _heap[a].CompareTo(_heap[b]);
    }

    private void Swap(int i, int j)
    {
        T temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;
    }

    private void DoubleHeapSize()
    {
        T[] copy = new T[_heap.Length * 2];

        for (int i = 0; i < _heap.Length; i++)
        {
            copy[i] = _heap[i];
            _heap[i] = default(T);
        }

        _heap = copy;
    }
    #endregion

    #region Methods - public
    public T Pop()
    {
        if (_count == 0)
        {
            throw new IndexOutOfRangeException($"Heap is empty");
        }

        T t = _heap[0];

        Swap(0, _count - 1);

        _count--;

        SiftDown(0, _count - 1);

        return t;
    }

    public void Push(T t)
    {
        if (_count == _heap.Length)
        {
            DoubleHeapSize();
        }

        _heap[_count] = t;
        _count++;

        SiftUp();
    }

    public void Fix()
    {
        int end = _count - 1;
        int lastNonLeafIndex = (_count - 2) / 2;
        for (int i = lastNonLeafIndex; i >= 0; i--)
        {
            SiftDown(i, end);
        }
    }

    public T GetAtIndex(int index)
    {
        if (index < 0 || index >= _count)
        {
            throw new IndexOutOfRangeException($"Index({index}) is out of bounds({_count})");
        }

        return _heap[index];
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < _count; i++)
        {
            s += $"{_heap[i]}, ";
        }

        return s;
    }
    #endregion
}
