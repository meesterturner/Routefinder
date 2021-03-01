# Routefinder
A basic C# library for pathfinding

## Introduction
This is a set pathfinding algorithms developed to used against points joined together as "roads". Demonstration program included.

## Usage
```
RouteFinder rf = new RouteFinder();

// Add roads - the values are x, y pairs for the start and end points
rf.AddRoad(0, 0, 0, 5);
rf.AddRoad(0, 0, 3, 0);
rf.AddRoad(3, 0, 4, 3);
rf.AddRoad(4, 3, 2, 1);

// Set the start and destination points
rf.StartX = 0;
rf.StartY = 0;
rf.DestinationX = 2;
rf.DestinationY = 1;

// Set one of three methods
rf.Method = FinderMethod.AStar; // Properly implemented A* (preferred)
rf.Method = FinderMethod.AStarImperfect; // Badly implemented A*, with some bugs, but works in most cases
rf.Method = FinderMethod.Dijkstra; // Dijkstra algorithm (least efficient)

// Start pathfinder
rf.Go();

// Check results
if (rf.Journeys.Count == 0)
{
    // Should have at least one journey, depending on method used
    MessageBox.Show("Unable to complete route");
}
else
{
    // Return the shortest journey
    // For A*, this will be the only journey in the list
    Journey j = rf.Shortest();
    
    // Returns the total distance of the journey
    int distance = j.Distance(); 
    
    // Returns the list of roads (including start and end points) in order
    List<Roads> r = j.Roads; 
}
```

## License
### MIT License
Copyright (c) 2021 Paul Turner

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.