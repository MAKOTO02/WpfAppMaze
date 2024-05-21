using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze2
{
    using System;

    public class DisjointSet
    {
        int universeSize;
        int[] parent;
        int[] rank;

        // Constructor to create and initialize sets of n items
        public DisjointSet(int universeSize)
        {
            this.universeSize = universeSize;
            this.parent = new int[universeSize];
            this.rank = new int[universeSize];
            for (int i = 0; i < universeSize; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        // Function to find the representative of the set containing u
        public int FindSet(int u)
        {
            if (u < 0 || u >= universeSize)
                throw new ArgumentOutOfRangeException();

            if (parent[u] != u)
            {
                parent[u] = FindSet(parent[u]); // Path compression
            }
            return parent[u];
        }

        // Function to union the sets containing u and v
        public void Union(int u, int v)
        {
            int rootU = FindSet(u);
            int rootV = FindSet(v);

            if (rootU != rootV)
            {
                // Union by rank
                if (rank[rootU] > rank[rootV])
                {
                    parent[rootV] = rootU;
                }
                else if (rank[rootU] < rank[rootV])
                {
                    parent[rootU] = rootV;
                }
                else
                {
                    parent[rootV] = rootU;
                    rank[rootU]++;
                }
            }
        }
    }

}
