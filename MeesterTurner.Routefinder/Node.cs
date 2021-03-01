using System;
using System.Collections.Generic;
using System.Text;

namespace MeesterTurner.Routefinder
{
    class Node
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int G { get; set; }
        public int H { get; set; }
        public int F { get; private set; }

        public Node PreviousNode { get; set; }

        public void CalculateF()
        {
            F = G + H;
        }

        public Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
