using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    interface IFinderMethod
    {
        int StartX { get; set; }
        int StartY { get; set; }
        int DestinationX { get; set; }
        int DestinationY { get; set; }
        List<Road> AllRoads { get; set; }
        List<Journey> GetJourneys();
    }
}
