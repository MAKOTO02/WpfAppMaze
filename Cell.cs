using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze2
{
    public class Cell
    {
        public EdgeFlags EdgeFlags;
        public CellState State;
        int id;
        public Cell(EdgeFlags edgeFlags = EdgeFlags.None, CellState state = CellState.None, int id = 0)
        {
            EdgeFlags = edgeFlags;
            State = state;
            this.id = id;
        }
    }

    public struct Edge
    {
        public int Source;
        public int Destination;
        public EdgeFlags Direction;

        public Edge(int source, int destination, EdgeFlags direction)
        {
            this.Source = source;
            this.Destination = destination;
            this.Direction = direction;
        }
    }

    [Flags]
    public enum EdgeFlags
    {
        None = 0,
        Left = 1,
        Down = 2,
        Right = 4,
        Up = 8,
        All = Left | Down | Right | Up
    }

    [Flags]
    public enum CellState
    {
        None = 0,
        IsStart = 1,
        IsEnd = 2,
        IsVisited = 4,
        InRoute = 8,
        // ここに属性を追加.
    }
}
