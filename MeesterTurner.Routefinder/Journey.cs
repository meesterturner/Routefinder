using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeesterTurner.Routefinder
{
    public class Journey
    {
        public List<Road> Roads = new List<Road>();
        public bool Success;

        public double Distance()
        {
            double dist = 0;

            foreach (Road r in Roads)
                dist += r.Distance();

            return dist;
        }
    }
}
