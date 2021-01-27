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

        public FinderMethod Method { get; set; } = FinderMethod.Dijkstra;

        private double shortestSuccess;
        private Dictionary<(int x, int y), int> nodeCost;

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
            shortestSuccess = -1;

            if (Method == FinderMethod.AStar)
                CalculateCosts();

            Build(StartX, StartY, new Journey());
        }

        private void CalculateCosts()
        {
            nodeCost = new Dictionary<(int x, int y), int>();
            foreach(Road r in allRoads)
            {
                (int x, int y) node = (-1, -1);
                for (int i = 0; i < 2; i++)
                {
                    node = (i == 0 ? r.From : r.To);
                
                    if(!nodeCost.ContainsKey(node))
                    {
                        int gridDistFromStart = Math.Abs(node.x - StartX) + Math.Abs(node.y - StartY);
                        int xDist = Convert.ToInt32(Math.Pow(node.x - DestinationX, 2));
                        int yDist = Convert.ToInt32(Math.Pow(node.y - DestinationY, 2));

                        int lineDistToEnd = (xDist + yDist); // Math.Sqrt

                        nodeCost[(node.x, node.y)] = gridDistFromStart + lineDistToEnd;
                    }
                }
            }
        }

        private void Build(int x, int y, Journey history)
        {
            if (Method == FinderMethod.AStar && journeys.Count > 1)
                return;

            List<CostedRoad> fromHere = new List<CostedRoad>();
            foreach(Road p in allRoads)
                if (p.From == (x, y) || p.To == (x, y))
                    fromHere.Add(new CostedRoad()
                    {
                        From = p.From,
                        To = p.To
                    });

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

            List<CostedRoad> nextPath = null;
            switch(Method)
            {
                case FinderMethod.Dijkstra:
                    nextPath = fromHere;
                    break;

                case FinderMethod.AStar:
                    for(int i = 0; i < fromHere.Count; i++)
                    {
                        CostedRoad f = fromHere[i];
                        if (f.From.x == x && f.From.y == y)
                            f.Cost = nodeCost[(f.To.x, f.To.y)];
                        else
                            f.Cost = nodeCost[(f.From.x, f.From.y)];
                    }

                    nextPath = (from CostedRoad cr in fromHere orderby cr.Cost select cr).ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid Method");
            }

            foreach(Road f in nextPath)
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

                    if(shortestSuccess == -1 || newPath.Distance() < shortestSuccess)
                        shortestSuccess = newPath.Distance();
                }
                else
                {
                    if (shortestSuccess != -1 && newPath.Distance() >= shortestSuccess)
                        break;
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
