using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MeesterTurner.Routefinder
{
    class AStarFinder : IFinderMethod
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }
        public List<Road> AllRoads { get; set; }

        private List<Node> openList;
        private List<Node> closedList;
        private List<Node> allNodes;

        private Node start = null;
        private Node end = null;

        public List<Journey> GetJourneys()
        {
            List<Journey> journeys = new List<Journey>();
            Journey j = Build();
            if (j != null)
                journeys.Add(j);

            return journeys;
        }

        private Journey Build()
        {
            // Got to give credit where it's due.....
            // It's basically a rip of https://www.youtube.com/watch?v=alU04hvz6L4
            // with some customisation work to make it work with my roads system

            Journey retVal = null;
            Setup();

            while (openList.Count > 0)
            {
                Node current = GetLowestFCost(openList);
                if (current == end)
                {
                    retVal = GetCompleteJourney();
                    break;
                }

                openList.Remove(current);
                closedList.Add(current);

                List<Node> neighbours = GetNeighbours(current);
                foreach(Node n in neighbours)
                {
                    if(closedList.Contains(n) == false)
                    {
                        int testG = n.G + DistanceCost(current, n);
                        if(testG < n.G)
                        {
                            n.PreviousNode = current;
                            n.G = testG;
                            n.H = DistanceCost(n, end);
                            n.CalculateF();

                            if(openList.Contains(n) == false)
                                openList.Add(n);
                        }
                    }
                }
            }

            // Run out of nodes (journeys to be left empty)
            return retVal;
        }

        /// <summary>
        /// Work backwards through the nodes, determinining the roads taken
        /// then assemble a journey in the correct order
        /// </summary>
        /// <returns></returns>
        private Journey GetCompleteJourney()
        {
            List<Road> roads = new List<Road>();

            Node current = end;

            while(current.PreviousNode != null)
            {
                // Inserting at the top so the roads are in the correct order!
                roads.Insert(0, FindRoad(current, current.PreviousNode));
                current = current.PreviousNode;
            }

            Journey retVal = new Journey();
            retVal.Success = true;
            retVal.Roads = roads;
            return retVal;
        }

        /// <summary>
        /// Find the appropriate road with a start and end point matching the two nodes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private Road FindRoad(Node a, Node b)
        {
            Road retVal = null;

            foreach(Road r in AllRoads)
            {
                if((r.From.x == a.X && r.From.y == a.Y && r.To.x == b.X && r.To.y == b.Y) ||
                    (r.To.x == a.X && r.To.y == a.Y && r.From.x == b.X && r.From.y == b.Y))
                {
                    retVal = r;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Setup some initial calculations for every node
        /// </summary>
        private void Setup()
        {
            allNodes = new List<Node>();

            Dictionary<(int x, int y), bool> Calculated = new Dictionary<(int x, int y), bool>();
            

            foreach(Road r in AllRoads)
            {
                (int x, int y) nodePoint;

                for (int i = 0; i < 2; i++)
                {
                    nodePoint = (i == 0 ? r.From : r.To);
                    
                    if (Calculated.ContainsKey(nodePoint) == false)
                    {
                        Node n = new Node(nodePoint.x, nodePoint.y);
                        n.G = int.MaxValue;
                        n.CalculateF();

                        if(n.X == StartX && n.Y == StartY)
                            start = n;

                        if (n.X == DestinationX && n.Y == DestinationY)
                            end = n;

                        allNodes.Add(n);
                        Calculated.Add(nodePoint, true);
                    }
                }
            }

            start.G = 0;
            start.H = DistanceCost(start, end);
            start.CalculateF();

            openList = new List<Node>();
            closedList = new List<Node>();

            openList.Add(start);
        }

        private List<Node> GetNeighbours(Node current)
        {
            List<Node> retVal = new List<Node>();

            foreach(Road r in AllRoads)
            {
                if (r.From.x == current.X && r.From.y == current.Y)
                    retVal.Add(NodeByPos(r.To.x, r.To.y));

                if (r.To.x == current.X && r.To.y == current.Y)
                    retVal.Add(NodeByPos(r.From.x, r.From.y));
            }

            return retVal;
        }

        private Node NodeByPos(int x, int y)
        {
            Node retVal = null;

            foreach(Node n in allNodes)
            {
                if(n.X == x && n.Y == y)
                {
                    retVal = n;
                    break;
                }
            }

            return retVal;
        }


        private Node GetLowestFCost(List<Node> nodes)
        {
            Node retVal = nodes[0];
            for(int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].F < retVal.F)
                    retVal = nodes[i];
            }

            return retVal;
        }

        private int DistanceCost(Node from, Node to)
        {
            int xDist = Math.Abs(from.X - to.X);
            int yDist = Math.Abs(from.Y - to.Y);
            //int remaining = Math.Abs(xDist - yDist);
            return xDist + yDist;

        }
    }
}
