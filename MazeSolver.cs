using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze2
{
    public  class MazeSolver
    {
        int width, height;
        int[] cost, parent;
        int start, end;
        Maze maze;
        public MazeSolver(int width, int height, GenerationMethod generationMethod)
        {
            this.width = width;
            this.height = height;
            this.start = 0;
            this.end = width * height - 1;
            cost = new int[width * height];
            parent = new int[width * height];
            for (int id = 0; id < cost.Length; id++)
            {
                cost[id] = -1;
                parent[id] = 0; // あとでstartのidに初期化.
            }      
            this.maze = new MazeGenerator(width, height).Generate(generationMethod);
            maze.ResetCellState();
            maze.SetCellState(start, CellState.IsStart);
            maze.SetCellState(end, CellState.IsEnd);
        }

        public Maze Solve(SolveMethod solveMethod, bool randomizeStartAndEnd = false, DistanceType type = DistanceType.L1)
        {
            if(randomizeStartAndEnd)
            {
                SetRandomStartAndEnd();
            }
            switch (solveMethod)
            {
                case SolveMethod.StabeleAstar:
                    Console.WriteLine(StableAstarSearch(type));
                    ReconstructPath();
                    return maze;
                case SolveMethod.FastAstar:
                    Console.WriteLine(FastAstarSearch(type));
                    ReconstructPath();
                    return maze;
                case SolveMethod.BFS:
                    ReconstructPath();
                    return maze;
                default: return maze;              
            }
        }
        public void SetRandomStartAndEnd()
        {
            Random rng = new Random();
            start = rng.Next(width * height);
            end = rng.Next(width * height);
            while(start == end)
            {
                end = rng.Next(width * height);
            }
            maze.ResetCellState();
            maze.SetCellState(start, CellState.IsStart);
            maze.SetCellState(end, CellState.IsEnd);
            for (int id = 0; id < parent.Length; id++)
            {
                parent[id] = start;
            }

        }

        public bool StableAstarSearch(DistanceType type)
        {
            bool endIsFound = false;
            int currentCell = start;
            cost[start] = 0;
            Func<int, int, int> distance = GetDistanceType(type);
            PriorityQueue<int, int> priorityQueue = new  PriorityQueue<int, int>(width * height);
            priorityQueue.Enqueue(StableAstar(distance, currentCell), currentCell);
            while (priorityQueue.Count > 0)
            {
                currentCell = priorityQueue.Dequeue();
                maze.SetCellState(currentCell, CellState.IsVisited);
                if ((maze.At(currentCell).State & CellState.IsEnd) > 0)
                {
                    endIsFound = true;
                    return endIsFound;
                }
                List<int> neigbors = OpenCell(currentCell);
                foreach(int cell in neigbors)
                {
                    cost[cell] = cost[currentCell] + 1;
                    parent[cell] = currentCell;
                    priorityQueue.Enqueue(StableAstar(distance, cell), cell);
                }
            }
            return endIsFound;
        }
        int StableAstar(Func<int, int, int> distance, int idx) => distance(idx, end) + cost[idx];

        public bool FastAstarSearch(DistanceType type)
        {
            bool endIsFound = false;
            int currentCell = start;
            Func<int, int, int> distance = GetDistanceType(type);
            VanEmdeBoasTree<int> veb = new VanEmdeBoasTree<int>(width * height);
            veb.Enqueue(FastAstar(distance, currentCell), currentCell);
            while (veb.Min >= 0)
            {
                currentCell = veb.ExtractMin();
                maze.SetCellState(currentCell, CellState.IsVisited);
                if ((maze.At(currentCell).State & CellState.IsEnd) > 0)
                {
                    endIsFound = true;
                    return endIsFound;
                }
                List<int> neigbors = OpenCell(currentCell);
                foreach(int cell in neigbors)
                {
                    cost[cell] = cost[currentCell] + 1;
                    parent[cell] = currentCell;
                    veb.Enqueue(FastAstar(distance, cell), cell);
                    
                }
                
            }
            return endIsFound;
        }

        int FastAstar(Func<int, int, int> distance, int idx) => Math.Min(distance(idx, end) + cost[idx], cost.Length);
        List<int> OpenCell(int cell)
        {
            EdgeFlags[] directions = { EdgeFlags.Left, EdgeFlags.Down, EdgeFlags.Right, EdgeFlags.Up };
            List<int> neighbors = new List<int>();
            EdgeFlags edgeFlags = maze.At(cell).EdgeFlags;
            foreach(EdgeFlags direction in directions)
            {
                if ((edgeFlags & direction) == 0)
                {
                    int neighbor = maze.Move(cell, direction);
                    if ((maze.At(neighbor).State & CellState.IsVisited) == 0)
                    {
                        neighbors.Add(neighbor);   
                    }
                }
            }  
            return neighbors;
        }

        void ReconstructPath()
        {
            int currentCell = end;
            while(currentCell != start)
            {
                maze.SetCellState(currentCell, CellState.InRoute);
                currentCell = parent[currentCell];
            }
            maze.SetCellState(start, CellState.InRoute);
        }

        Func<int, int, int> GetDistanceType(DistanceType type)
        {
            switch (type)
            {
                case DistanceType.L1:
                    return L1;
                case DistanceType .L2:
                    return L2;
                default:
                    return (int first, int second) => 0;
            }
        }
        int L1(int first, int second)
        {
            return Math.Abs(first/width - second/width) + Math.Abs((first - second) % width);
        }

        int L2(int first, int second)
        {
            int dy = (first - second) / width;
            int dx = (first / width - second / width);
            return dx * dx + dy * dy;
        }
        
    }

    public enum SolveMethod 
    { 
        StabeleAstar,
        FastAstar,
        BFS,
    }

    public enum DistanceType
    {
        L1,
        L2,
    }
}
