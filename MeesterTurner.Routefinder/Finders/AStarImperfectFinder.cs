using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    class AStarImperfectFinder : IFinderMethod
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }
        public List<Road> AllRoads { get; set; }

        private double shortestSuccess;
        private List<Journey> journeys;
        private Dictionary<(int x, int y), int> nodeCost;

        public List<Journey> GetJourneys()
        {

            journeys = new List<Journey>();

            shortestSuccess = -1;
            CalculateCosts();
            Build(StartX, StartY, new Journey());

            return journeys;
        }

        private void Build(int x, int y, Journey history)
        {
            if (journeys.Count > 1)
                return;

            List<CostedRoad> fromHere = new List<CostedRoad>();
            foreach (Road p in AllRoads)
                if (p.From == (x, y) || p.To == (x, y))
                    fromHere.Add(new CostedRoad()
                    {
                        From = p.From,
                        To = p.To
                    });


            foreach (Road fh in fromHere)
            {
                (int x, int y) nodeAtEndOfRoad;
                if (fh.From.x == x && fh.From.y == y)
                    nodeAtEndOfRoad = fh.To;
                else
                    nodeAtEndOfRoad = fh.From;

                foreach (Road histRoad in history.Roads)
                {

                    if (
                        (fh.To == histRoad.To && fh.From == histRoad.From) ||
                        (fh.To == histRoad.From && fh.From == histRoad.To) ||
                        (histRoad.From == nodeAtEndOfRoad || histRoad.To == nodeAtEndOfRoad)
                        )
                    {
                        fh.Taken = true;
                        break;
                    }
                }
            }

            for (int p = fromHere.Count - 1; p >= 0; p--)
            {
                if (fromHere[p].Taken)
                    fromHere.RemoveAt(p);
            }

            List<CostedRoad> nextPath = null;
            for (int i = 0; i < fromHere.Count; i++)
            {
                CostedRoad f = fromHere[i];
                if (f.From.x == x && f.From.y == y)
                    f.Cost = nodeCost[(f.To.x, f.To.y)];
                else
                    f.Cost = nodeCost[(f.From.x, f.From.y)];
            }

            nextPath = (from CostedRoad cr in fromHere orderby cr.Cost select cr).ToList();

            foreach (Road f in nextPath)
            {
                Journey newPath = new Journey();
                newPath.Roads = CopyRoutes(history.Roads);
                newPath.Roads.Add(f);

                int newX;
                int newY;

                if (f.From == (x, y))
                {
                    newX = f.To.x;
                    newY = f.To.y;
                }
                else
                {
                    newX = f.From.x;
                    newY = f.From.y;
                }

                if (newX == DestinationX && newY == DestinationY)
                {
                    newPath.Success = true;
                    journeys.Add(newPath);

                    if (shortestSuccess == -1 || newPath.Distance() < shortestSuccess)
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

        private void CalculateCosts()
        {
            nodeCost = new Dictionary<(int x, int y), int>();
            foreach (Road r in AllRoads)
            {
                (int x, int y) node = (-1, -1);
                for (int i = 0; i < 2; i++)
                {
                    node = (i == 0 ? r.From : r.To);

                    if (!nodeCost.ContainsKey(node))
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
        private List<Road> CopyRoutes(List<Road> paths)
        {
            List<Road> retVal = new List<Road>();
            foreach (Road r in paths)
                retVal.Add(new Road(r.From, r.To));

            return retVal;
        }
    }
}
