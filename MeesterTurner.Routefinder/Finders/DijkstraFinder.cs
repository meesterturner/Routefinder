using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    class DijkstraFinder : IFinderMethod
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }
        public List<Road> AllRoads { get; set; }

        private double shortestSuccess;
        private List<Journey> journeys;

        List<Journey> IFinderMethod.GetJourneys()
        {
            journeys = new List<Journey>();

            shortestSuccess = -1;
            Build(StartX, StartY, new Journey());

            return journeys;
        }

        private void Build(int x, int y, Journey history)
        {
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
                        (fh.To == histRoad.From && fh.From == histRoad.To)
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

            List<CostedRoad> nextPath = fromHere;
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

        private List<Road> CopyRoutes(List<Road> paths)
        {
            List<Road> retVal = new List<Road>();
            foreach (Road r in paths)
                retVal.Add(new Road(r.From, r.To));

            return retVal;
        }
    }
}
