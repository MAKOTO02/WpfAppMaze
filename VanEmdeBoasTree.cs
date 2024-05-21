using System;
using System.Collections.Generic;

namespace SimpleMaze2
{
    public class VanEmdeBoasTree
    {
        int minimum;
        int maximum;
        readonly int lowerRoot;
        readonly int upperRoot;
        public readonly int universeSize;
        public readonly VanEmdeBoasTree summary;
        public readonly Dictionary<int, VanEmdeBoasTree> cluster;

        // Constructor
        public VanEmdeBoasTree(int capacity)
        {
            this.universeSize = Upper(capacity);
            this.lowerRoot = LowerRoot(universeSize);
            this.upperRoot = universeSize / lowerRoot;
            this.minimum = -1;
            this.maximum = -1;

            if (this.universeSize <= 2)
            {
                summary = null;
                cluster = null;
            }
            else
            {
                summary = new VanEmdeBoasTree(upperRoot);
                cluster = new Dictionary<int, VanEmdeBoasTree>();
            }
        }

        public int Min() => this.minimum;

        public int Max() => this.maximum;

        public bool Member(int x)
        {
            if (x == this.minimum || x == this.maximum) return true;
            else if (this.universeSize == 2) return false;
            else if (cluster.ContainsKey(High(x)))
                return cluster[High(x)].Member(Low(x));
            return false;
        }

        public int Successor(int x)
        {
            int high = High(x);
            int low = Low(x);
            if (universeSize == 2)
            {
                if (x == 0 && maximum == 1) return 1;
                else return -1;
            }
            else if (minimum >= 0 && x < minimum) return minimum;
            else
            {
                if (cluster.ContainsKey(high))
                {
                    int maxLow = cluster[high].Max();
                    if (maxLow >= 0 && low < maxLow)
                    {
                        int offset = cluster[high].Successor(low);
                        return Index(high, offset);
                    }
                }
                int succCluster = summary.Successor(high);
                if (succCluster < 0) return -1;
                else
                {
                    int offset = cluster[succCluster].Min();
                    return Index(succCluster, offset);
                }
            }
        }

        public int Predecessor(int x)
        {
            int high = High(x);
            int low = Low(x);
            if (universeSize == 2)
            {
                if (x == 1 && minimum == 0) return 0;
                else return -1;
            }
            else if (maximum >= 0 && x > maximum) return maximum;
            else
            {
                if (cluster.ContainsKey(high))
                {
                    int minLow = cluster[high].Min();
                    if (minLow >= 0 && low > minLow)
                    {
                        int offset = cluster[high].Predecessor(low);
                        return Index(high, offset);
                    }
                }
                int predCluster = summary.Predecessor(high);
                if (predCluster < 0)
                {
                    if (minimum >= 0 && x > minimum) return minimum;
                    else return -1;
                }
                else
                {
                    int offset = cluster[predCluster].Max();
                    return Index(predCluster, offset);
                }
            }
        }

        void EmptyInsert(int x)
        {
            this.minimum = x;
            this.maximum = x;
        }

        public void Insert(int x)
        {
            if (this.minimum < 0) this.EmptyInsert(x);
            else
            {
                if (x < this.minimum)
                {
                    (this.minimum, x) = (x, this.minimum);
                }
                if (this.universeSize > 2)
                {
                    int high = High(x);
                    int low = Low(x);
                    if (!cluster.ContainsKey(high))
                    {
                        cluster[high] = new VanEmdeBoasTree(lowerRoot);
                    }
                    if (cluster[high].Min() < 0)
                    {
                        summary.Insert(high);
                        cluster[high].EmptyInsert(low);
                    }
                    else
                    {
                        cluster[high].Insert(low);
                    }
                }
                if (x > this.maximum)
                {
                    this.maximum = x;
                }
            }
        }

        public void Delete(int x)
        {
            if (this.minimum == this.maximum)
            {
                this.minimum = -1;
                this.maximum = -1;
            }
            else if (universeSize == 2)
            {
                if (x == 0) this.minimum = 1;
                else this.minimum = 0;
                this.maximum = this.minimum;
            }
            else
            {
                if (x == this.minimum)
                {
                    int firstCluster = this.summary.Min();
                    x = Index(firstCluster, this.cluster[firstCluster].Min());
                    this.minimum = x;
                }
                int high = High(x);
                int low = Low(x);
                if (cluster.ContainsKey(high))
                {
                    cluster[high].Delete(low);
                    if (cluster[high].Min() < 0)
                    {
                        this.summary.Delete(high);
                        cluster.Remove(high);
                        if (x == this.maximum)
                        {
                            int summaryMax = this.summary.Max();
                            if (summaryMax < 0) this.maximum = this.minimum;
                            else this.maximum = Index(summaryMax, this.cluster[summaryMax].Max());
                        }
                    }
                    else if (x == this.maximum)
                    {
                        this.maximum = Index(high, this.cluster[high].Max());
                    }
                }
            }
        }

        static public int HigherLog2(int value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            return (int)Math.Ceiling(Math.Log(value, 2));
        }

        int Upper(int x) => 1 << HigherLog2(x);

        static public int LowerRoot(int x) => 1 << (HigherLog2(x) / 2);

        public int High(int x) => x / this.lowerRoot;

        public int Low(int x) => x % this.lowerRoot;

        int Index(int x, int y) => x * this.lowerRoot + y;
    }

    public class VanEmdeBoasTree<TElement>
    {
        VanEmdeBoasTree vanEmdeBoasTree;
        public Dictionary<int, Stack<TElement>> dataDictionary;
        public Dictionary<int, int> Quantity;
        public int Count { get; private set; }
        public int universeSize;

        public VanEmdeBoasTree(int capacity)
        {
            vanEmdeBoasTree = new VanEmdeBoasTree(capacity);
            this.universeSize = vanEmdeBoasTree.universeSize;
            Count = 0;
            dataDictionary = new Dictionary<int, Stack<TElement>>();
            Quantity = new Dictionary<int, int>();
        }

        public void Enqueue(int key, TElement element)
        {
            if (key < 0 || key > universeSize - 1) return;
            Count++;
            if (!Quantity.ContainsKey(key) || Quantity[key] == 0)
            {
                vanEmdeBoasTree.Insert(key);
                Quantity[key] = 0;
            }
            if (!dataDictionary.ContainsKey(key))
            {
                dataDictionary[key] = new Stack<TElement>();
            }
            dataDictionary[key].Push(element);
            Quantity[key]++;
        }

        public TElement Remove(int key)
        {
            Count--;
            if (Quantity.ContainsKey(key) && Quantity[key] == 1)
                vanEmdeBoasTree.Delete(key);

            if (Quantity.ContainsKey(key) && Quantity[key] > 0)
            {
                Quantity[key]--;
                return dataDictionary[key].Pop();
            }
            else
            {
                return default;
            }
        }

        public TElement ExtractMax() => Remove(vanEmdeBoasTree.Max());

        public TElement ExtractMin() => Remove(vanEmdeBoasTree.Min());

        public int Max => vanEmdeBoasTree.Max();

        public int Min => vanEmdeBoasTree.Min();

        public bool Contains(int x) => vanEmdeBoasTree.Member(x);
    }
}
