using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze2
{
    class HeapNode<TKey, TElement> : IComparable<HeapNode<TKey, TElement>> where TKey : IComparable<TKey>
    {
        public TKey Key { get; }
        public TElement Element { get; }

        public HeapNode(TKey key, TElement element)
        {
            Key = key;
            Element = element;
        }

        public int CompareTo(HeapNode<TKey, TElement> other)
        {
            return Key.CompareTo(other.Key);
        }
    }
    public interface IInfinityProvider<T>
    {
        T NegativeInfinity { get; }
        T PositiveInfinity { get; }
    }

    public class IntInfinityProvider : IInfinityProvider<int>
    {
        public int NegativeInfinity => int.MinValue;
        public int PositiveInfinity => int.MaxValue;
    }

    public class FloatInfinityProvider : IInfinityProvider<float>
    {
        public float NegativeInfinity => float.NegativeInfinity;
        public float PositiveInfinity => float.PositiveInfinity;
    }

    public class DoubleInfinityProvider : IInfinityProvider<double>
    {
        public double NegativeInfinity => double.NegativeInfinity;
        public double PositiveInfinity => double.PositiveInfinity;
    }

    class HeapNodeInfinityProvider<TKey, TElement> : IInfinityProvider<HeapNode<TKey, TElement>> where TKey : IComparable<TKey>
    {
        public HeapNode<TKey, TElement> NegativeInfinity
        {
            get
            {
                if (typeof(TKey) == typeof(int)) return new HeapNode<TKey, TElement>((TKey)(object)int.MinValue, (TElement)default);
                if (typeof(TKey) == typeof(float)) return new HeapNode<TKey, TElement>((TKey)(object)float.NegativeInfinity, (TElement)default);
                if (typeof(TKey) == typeof(double)) return new HeapNode<TKey, TElement>((TKey)(object)double.NegativeInfinity, (TElement)default);
                throw new NotSupportedException($"Infinity provider is not supported for type{typeof(TKey)}");
            }
        }
        public HeapNode<TKey, TElement> PositiveInfinity
        {
            get
            {
                if (typeof(TKey) == typeof(int)) return new HeapNode<TKey, TElement>((TKey)(object)int.MaxValue, (TElement)default);
                if (typeof(TKey) == typeof(float)) return new HeapNode<TKey, TElement>((TKey)(object)float.PositiveInfinity, (TElement)default);
                if (typeof(TKey) == typeof(double)) return new HeapNode<TKey, TElement>((TKey)(object)double.PositiveInfinity, (TElement)default);
                throw new NotSupportedException($"Infinity provider is not supported for type{typeof(TKey)}");
            }
        }
    }

    public class BinaryHeap<TKey> where TKey : IComparable<TKey>
    {
        IInfinityProvider<TKey> InfinityProvider;
        int maxSize;
        TKey[] heap;
        int heapSize;

        public BinaryHeap(int maxSize = 128)
        {
            this.InfinityProvider = GetInfinityProvider();
            this.maxSize = maxSize;
            this.heap = new TKey[maxSize];
            this.heapSize = 0;
        }

        IInfinityProvider<TKey> GetInfinityProvider()
        {
            if (typeof(TKey) == typeof(int)) return (IInfinityProvider<TKey>)new IntInfinityProvider();
            if (typeof(TKey) == typeof(float)) return (IInfinityProvider<TKey>)new FloatInfinityProvider();
            if (typeof(TKey) == typeof(double)) return (IInfinityProvider<TKey>)new DoubleInfinityProvider();
            if (typeof(TKey).IsGenericType && typeof(TKey).GetGenericTypeDefinition() == typeof(HeapNode<,>))
            {
                Type[] genericArguments = typeof(TKey).GetGenericArguments();
                Type providerType = typeof(HeapNodeInfinityProvider<,>).MakeGenericType(genericArguments);
                return (IInfinityProvider<TKey>)Activator.CreateInstance(providerType);
            }
            throw new NotSupportedException($"Infinity provider is not supported for type{typeof(TKey)}");
        }
        int Parent(int idx)
        {
            return (idx - 1) / 2;
        }

        int Left(int idx)
        {
            return 2 * idx + 1;
        }

        int Right(int idx)
        {
            return 2 * idx + 2;
        }

        void MaxHeapify(int idx)
        {
            int l = Left(idx);
            int r = Right(idx);
            int largest; // 親と二つの子の中から最大の要素のidxをlargestに保管する. 
            if (l <= this.heapSize - 1 && heap[l].CompareTo(heap[idx]) > 0) largest = l;
            else largest = idx;
            if (r <= heapSize - 1 && heap[r].CompareTo(heap[largest]) > 0) largest = r;
            if (largest != idx)
            {
                (heap[idx], heap[largest]) = (heap[largest], heap[idx]);    // swap
                MaxHeapify(largest);
            }
        }
        void MinHeapify(int idx)
        {
            int l = Left(idx);
            int r = Right(idx);
            int smallest;
            if (l <= this.heapSize - 1 && heap[l].CompareTo(heap[idx]) < 0) smallest = l;
            else smallest = idx;
            if (r <= heapSize - 1 && heap[r].CompareTo(heap[smallest]) < 0) smallest = r;
            if (smallest != idx)
            {
                (heap[idx], heap[smallest]) = (heap[smallest], heap[idx]);    // swap
                MaxHeapify(smallest);
            }
        }
        public TKey[] BuildMaxHeap(TKey[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                MaxHeapInsert(elements[i]);
            }
            return this.heap;
        }

        public TKey[] BuildMinHeap(TKey[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                MinHeapInsert(elements[i]);
            }
            return this.heap;
        }

        public TKey Max()
        {
            if (this.heapSize <= 0) throw new InvalidOperationException("Heap is empty");
            return heap[0];
        }

        public TKey Min()
        {
            if (this.heapSize <= 0) throw new InvalidOperationException("Heap is empty");
            return heap[0];
        }

        public TKey ExtractMax()
        {
            if (this.heapSize < 1) throw new InvalidOperationException("Heap is empty");
            TKey max = heap[0];
            heap[0] = heap[heapSize - 1];
            this.heapSize--;
            MaxHeapify(0);
            return max;
        }

        public TKey ExtractMin()
        {
            if (this.heapSize < 1) throw new InvalidOperationException("Heap is empty");
            TKey min = heap[0];
            heap[0] = heap[heapSize - 1];
            this.heapSize--;
            MinHeapify(0);
            return min;
        }

        void IncreaseKey(int idx, TKey key)
        {
            if (key.CompareTo(heap[idx]) < 0) throw new Exception("Given key is smaller than current value");
            heap[idx] = key;
            while (idx > 0 & heap[Parent(idx)].CompareTo(heap[idx]) < 0)
            {
                (heap[idx], heap[Parent(idx)]) = (heap[Parent(idx)], heap[idx]);
                idx = Parent(idx);
            }
        }

        void DecreaseKey(int idx, TKey key)
        {
            if (key.CompareTo(heap[idx]) > 0) throw new Exception("Given key is largerer than current value");
            heap[idx] = key;
            while (idx > 0 & heap[Parent(idx)].CompareTo(heap[idx]) > 0)
            {
                (heap[idx], heap[Parent(idx)]) = (heap[Parent(idx)], heap[idx]);
                idx = Parent(idx);
            }
        }

        public void MaxHeapInsert(TKey key)
        {
            if (this.heapSize >= this.maxSize) throw new InvalidOperationException("Heap is full");
            this.heapSize++;
            heap[this.heapSize - 1] = InfinityProvider.NegativeInfinity;
            IncreaseKey(heapSize - 1, key);
        }

        public void MinHeapInsert(TKey key)
        {
            if (this.heapSize >= this.maxSize) throw new InvalidOperationException("Heap is full");
            this.heapSize++;
            heap[this.heapSize - 1] = InfinityProvider.PositiveInfinity;
            DecreaseKey(heapSize - 1, key);
        }

        public int Count => heapSize;
    }

    // min優先度付きのQueueを定義します.
    public class PriorityQueue<TKey, TElement> where TKey : IComparable<TKey>
    {
        int maxSize;
        private BinaryHeap<HeapNode<TKey, TElement>> _heap;
        public Type typeofKey => typeof(TKey);
        public Type typeofTElement => typeof(TElement);
        public PriorityQueue(int maxSize = 128)
        {
            this.maxSize = maxSize;
            _heap = new BinaryHeap<HeapNode<TKey, TElement>>(maxSize);

        }
        public void Enqueue(TKey key, TElement element)
        {
            var node = new HeapNode<TKey, TElement>(key, element);
            _heap.MinHeapInsert(node);
        }

        public TElement Dequeue()
        {
            return _heap.ExtractMin().Element;
        }

        public TElement Peek()
        {
            return _heap.Min().Element;
        }

        public int Count => _heap.Count;
    }
}
