using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze2
{
    public class MazeGenerator
    {
        int width, height;
        Maze maze;

        public MazeGenerator(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.maze = new Maze(width, height);
        }

        public Maze Generate(GenerationMethod generationMethod)
        {
            switch (generationMethod)
            {
                case (GenerationMethod.Kruskal):
                    return Kruskal();
                case (GenerationMethod.Prim):
                    return Prim();
                default:
                    return maze;
            }
        }
        Maze Prim()
        {
            List<Edge> candidateEdges = new List<Edge>();
            Random rng = new Random();

            int startX = rng.Next(width);
            int startY = rng.Next(height);
            int startId = maze.Index(startX, startY);
            maze.At(startId).State = CellState.IsVisited;

            AddEdges(candidateEdges, startId);

            while (candidateEdges.Count > 0)
            {
                int index = rng.Next(candidateEdges.Count);
                Edge edge = candidateEdges[index];
                candidateEdges.RemoveAt(index);

                if (maze.At(edge.Destination).State != CellState.IsVisited)
                {
                    maze.RemoveWall(edge.Source, edge.Direction);
                    maze.At(edge.Destination).State = CellState.IsVisited;
                    AddEdges(candidateEdges, edge.Destination);
                }
            }

            return maze;
        }

        void AddEdges(List<Edge> edges, int id)
        {
            foreach (EdgeFlags direction in Enum.GetValues(typeof(EdgeFlags)))
            {
                if (direction == EdgeFlags.None || direction == EdgeFlags.All)
                    continue;
                try
                {
                    int neighborId = maze.Move(id, direction);
                    if (maze.At(neighborId).State != CellState.IsVisited)
                    {
                        edges.Add(new Edge(id, neighborId, direction));
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore invalid moves
                }
            }
        }

        public Maze Kruskal()
        {
            DisjointSet disjointSet = new DisjointSet(width * height);
            List<int> allEdges = GetAllEdgesList();
            Shuffle(allEdges);

            foreach (int edge in allEdges)
            {
                int id = edge / 4;
                EdgeFlags edgeFlags;
                if (edge % 4 == 0 && id % width > 0) edgeFlags = EdgeFlags.Left;
                else if (edge % 4 == 1 && id / width < height - 1) edgeFlags = EdgeFlags.Down;
                else if (edge % 4 == 2 && id % width < width - 1) edgeFlags = EdgeFlags.Right;
                else if (edge % 4 == 3 && id / width > 0) edgeFlags = EdgeFlags.Up;
                else continue;

                int connectingId = maze.Move(id, edgeFlags);
                if (disjointSet.FindSet(id) != disjointSet.FindSet(connectingId))
                {
                    disjointSet.Union(id, connectingId);
                    maze.RemoveWall(id, edgeFlags);
                }
            }
            return maze;
        }

        public List<int> GetAllEdgesList()
        {
            List<int> allEdges = new List<int>();
            for (int edgeId = 0; edgeId < width * height * 4; edgeId++) allEdges.Add(edgeId);
            return allEdges;
        }

        public void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public enum GenerationMethod 
    {
        Kruskal,
        Prim
    }

}
