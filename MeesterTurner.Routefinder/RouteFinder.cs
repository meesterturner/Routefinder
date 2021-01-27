using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    public class RouteFinder
    {
        public List<Road> allRoads { get; set; } = new List<Road>();
        public List<Journey> journeys { get; set; }

        public int StartX { get; set; }
        public int StartY { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }

        public void AddRoad(int fromX, int fromY, int toX, int toY)
        {
            allRoads.Add(new Road(fromX, fromY, toX, toY));
        }

        public Journey Shortest()
        {
            return (from Journey j in journeys
                    orderby j.Distance()
                    select j).First();
        }

        public void Go()
        {
            if (StartX == DestinationX && StartY == DestinationY)
                throw new ArgumentException("Start and Destination points are the same");

            journeys = new List<Journey>();
            Build(StartX, StartY, new Journey());
        }

        private void Build(int x, int y, Journey history)
        {
            List<Road> fromHere = new List<Road>();
            foreach(Road p in allRoads)
                if (p.From == (x, y) || p.To == (x, y))
                    fromHere.Add(new Road(p.From, p.To));

            foreach(Road r in history.Roads)
            {
                foreach(Road fh in fromHere)
                {
                    if(
                        (fh.To == r.To && fh.From == r.From) ||
                        (fh.To == r.From && fh.From == r.To)
                        )
                    {
                        fh.Taken = true;
                    }
                }
            }

            for (int p = fromHere.Count - 1; p >= 0; p--)
            {
                if (fromHere[p].Taken)
                    fromHere.RemoveAt(p);
            }

            foreach(Road f in fromHere)
            {
                Journey newPath = new Journey();
                newPath.Roads = CopyRoutes(history.Roads);
                newPath.Roads.Add(f);

                int newX;
                int newY;

                if(f.From == (x, y))
                {
                    newX = f.To.x;
                    newY = f.To.y;
                } else
                {
                    newX = f.From.x;
                    newY = f.From.y;
                }

                if(newX == DestinationX && newY == DestinationY)
                {
                    newPath.Success = true;
                    journeys.Add(newPath);
                }
                else
                {
                    Build(newX, newY, newPath);
                }
            }

            // If no more paths from here, it should exit

        }

        private List<Road> CopyRoutes(List<Road> paths)
        {
            List<Road> retVal = new List<Road>();
            foreach(Road r in paths)
                retVal.Add(new Road(r.From, r.To));

            return retVal;
        }
    }
}
