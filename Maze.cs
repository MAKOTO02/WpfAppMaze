using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze2
{
    public class Maze
    {
        int width, height;
        Cell[,] grid;

        public Maze(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.grid = new Cell[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.grid[y, x] = new Cell(EdgeFlags.All, CellState.None, Index(x, y));
                }
            }
        }

        public int Index(int x, int y)
        {
            return x + y * width;
        }

        public int Move(int id, EdgeFlags edgeFlags)
        {
            int x = id % width;
            int y = id / width;
            if (edgeFlags == EdgeFlags.Left && x > 0)
            {
                return id - 1;
            }
            else if (edgeFlags == EdgeFlags.Right && x < width - 1)
            {
                return id + 1;
            }
            else if (edgeFlags == EdgeFlags.Down && y < height - 1)
            {
                return id + width;
            }
            else if (edgeFlags == EdgeFlags.Up && y > 0)
            {
                return id - width;
            }
            else throw new ArgumentException("EdgeFlags is invalid or move out of bounds.");
        }

        public Cell At(int id)
        {
            return grid[id / width, id % width];
        }

        public void SetWall(int id, EdgeFlags edgeFlags)
        {
            At(id).EdgeFlags |= edgeFlags;
            int neighborId = Move(id, edgeFlags);
            EdgeFlags oppositeEdge = Opposite(edgeFlags);
            At(neighborId).EdgeFlags |= oppositeEdge;
        }

        public void RemoveWall(int id, EdgeFlags edgeFlags)
        {
            At(id).EdgeFlags &= ~edgeFlags;
            int neighborId = Move(id, edgeFlags);
            EdgeFlags oppositeEdge = Opposite(edgeFlags);
            At(neighborId).EdgeFlags &= ~oppositeEdge;
        }

        EdgeFlags Opposite(EdgeFlags edgeFlags)
        {
            switch (edgeFlags)
            {
                case EdgeFlags.Left: return EdgeFlags.Right;
                case EdgeFlags.Right: return EdgeFlags.Left;
                case EdgeFlags.Up: return EdgeFlags.Down;
                case EdgeFlags.Down: return EdgeFlags.Up;
                default: throw new ArgumentException("Invalid edge flag.");
            }
        }

        public void ResetCellState()
        {
            for (int id = 0; id < grid.Length; id++)
            {
                At(id).State = CellState.None;
            }
        }

        public void SetCellState(int id, CellState state)
        {
            At(id).State |= state;
        }

        public void PrintMaze(bool ShowVisited = false)
        {
            // 一番上の壁
            for (int x = 0; x < width; x++)
            {
                Console.Write("+---");
            }
            Console.Write("+" + Environment.NewLine);

            for (int y = 0; y < height; y++)
            {
                // 縦の壁とスペース
                for (int x = 0; x < width; x++)
                {
                    if (x == 0)
                    {
                        Console.Write("|");
                    }
                    if((grid[y, x].State & CellState.IsStart) > 0)
                    {
                        Console.Write(" S ");
                    }
                    else if ((grid[y, x].State & CellState.IsEnd) > 0)
                    {
                        Console.Write(" E ");
                    }
                    else if((grid[y, x].State & CellState.InRoute) > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(" O ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (ShowVisited && (grid[y,x].State&CellState.IsVisited) > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" X ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    if ((grid[y, x].EdgeFlags & EdgeFlags.Right) != 0) Console.Write("|");
                    else Console.Write(" ");
                }
                Console.Write(Environment.NewLine);
                

                // 横の壁
                for (int x = 0; x < width; x++)
                {
                    Console.Write("+");
                    if ((grid[y, x].EdgeFlags & EdgeFlags.Down) != 0) Console.Write("---");
                    else Console.Write("   ");
                }
                Console.Write("+" +Environment.NewLine);
                
            }
        }
    }

}
