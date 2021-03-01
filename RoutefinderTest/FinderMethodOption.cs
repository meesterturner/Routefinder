using MeesterTurner.Routefinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfinderTest
{
    class FinderMethodOption
    {
        public string DisplayName { get; set; }
        public FinderMethod Method { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

        public FinderMethodOption(string displayName, FinderMethod method)
        {
            DisplayName = displayName;
            Method = method;
        }
    }
}
