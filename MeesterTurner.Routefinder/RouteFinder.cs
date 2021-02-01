using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    public class RouteFinder
    {
        public List<Road> AllRoads { get; set; } = new List<Road>();
        public List<Journey> Journeys { get; set; }

        public int StartX { get; set; }
        public int StartY { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }

        public FinderMethod Method { get; set; } = FinderMethod.Dijkstra;

        public void AddRoad(int fromX, int fromY, int toX, int toY)
        {
            AllRoads.Add(new Road(fromX, fromY, toX, toY));
        }

        public Journey Shortest()
        {
            return (from Journey j in Journeys
                    orderby j.Distance()
                    select j).First();
        }

        public void Go()
        {
            if (StartX == DestinationX && StartY == DestinationY)
                throw new ArgumentException("Start and Destination points are the same");

            if (AllRoads.Count == 0)
                throw new Exception("No roads have been specified");

            IFinderMethod finder = null;
            switch(Method)
            {
                case FinderMethod.Dijkstra:
                    finder = new DijkstraFinder();
                    break;

                case FinderMethod.AStarImperfect:
                    finder = new AStarImperfectFinder();
                    break;

                case FinderMethod.AStar:
                    throw new NotImplementedException();
            }

            finder.StartX = StartX;
            finder.StartY = StartY;
            finder.DestinationX = DestinationX;
            finder.DestinationY = DestinationY;
            finder.AllRoads = AllRoads;
            Journeys = finder.GetJourneys();

        }
    }
}
