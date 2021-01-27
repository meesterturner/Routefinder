using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    public class Road
    {
        public (int x, int y) From;
        public (int x, int y) To;
        public bool Taken;

        public double Distance()
        {
            double xDist = Math.Pow(From.x - To.x, 2);
            double yDist = Math.Pow(From.y - To.y, 2);

            return Math.Sqrt(xDist + yDist);
        }

        public Road()
        {

        }

        public Road(int fromX, int fromY, int toX, int toY)
        {
            From.x = fromX;
            From.y = fromY;
            To.x = toX;
            To.y = toY;
        }

        public Road((int x, int y) from, (int x, int y) to)
        {
            From.x = from.x;
            From.y = from.y;
            To.x = to.x;
            To.y = to.y;
        }
    }
}
