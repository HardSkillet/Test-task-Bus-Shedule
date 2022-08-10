using System;
using System.Collections.Generic;
using System.IO;

namespace BusSchedule
{
    public static class Program
    {
        
        const Int32 upperTime = 1440;
        static Int32 N;
        static Int32 K;
        static MinTime[] minTimes;
        static MinCost[] minCosts;
        static Bus[] buses;
        static Int32 start;
        static Int32 finish;
        static Int32 startTime;

        public class MinTime {
            public Boolean isMin = false;
            public Int32 currentMin = upperTime;
            public List<RouteMap> route = new List<RouteMap>();
        }

        public class MinCost {
            public Boolean isMin = false;
            public Int32 currentMin = Int32.MaxValue;
            public Int32 time = upperTime;
            public List<RouteMap> route = new List<RouteMap>();
        }

        static void ReadData(String path) {
            using (StreamReader fs = new StreamReader(path))
            {
                N = Int32.Parse(fs.ReadLine());
                K = Int32.Parse(fs.ReadLine());

                minTimes = new MinTime[K];
                minCosts = new MinCost[K];
                for (Int32 i = 0; i < K; i++) {
                    minTimes[i] = new MinTime();
                    minCosts[i] = new MinCost();
                }
                buses = new Bus[N];

                var temp = fs.ReadLine().Split();

                for (Int32 i = 0; i < N; i++)
                {
                    var times = temp[i].Split(':');
                    buses[i] = new Bus(i, Int32.Parse(times[0]), Int32.Parse(times[1]));
                }

                temp = fs.ReadLine().Split();

                for (Int32 i = 0; i < N; i++)
                {
                    buses[i].Cost = Int32.Parse(temp[i]);
                }

                for (Int32 i = 0; i < N; i++)
                {
                    temp = fs.ReadLine().Split();
                    var numberOfStops = Int32.Parse(temp[0]);
                    buses[i].RouteInitialization(numberOfStops);

                    for (Int32 j = 0; j < numberOfStops; j++)
                    {
                        buses[i][j] = new StopTime(Int32.Parse(temp[j + 1]) - 1);
                    }

                    for (Int32 j = 0; j < numberOfStops; j++)
                    {
                        buses[i][j].When = buses[i].DepartureTime + buses[i].stopCycle;
                        buses[i].stopCycle += Int32.Parse(temp[j + 1 + numberOfStops]);
                    }
                }
                var t = fs.ReadLine();
                if (t is null)
                {
                    start = 1; finish = K; startTime = 0;
                }
                else {
                    start = Int32.Parse(t);
                    t = fs.ReadLine();
                    finish = Int32.Parse(t);
                    temp = fs.ReadLine().Split(':');
                    startTime = 60 * Int32.Parse(temp[0]) + Int32.Parse(temp[1]);
                }
            }
        }
        public static void FindMinTime(Int32 start, Int32 finish, Int32 startTime)
        {
            StopTime currentStartStop = new StopTime(start - 1, minTimes[start - 1].currentMin);

            while (!minTimes[finish - 1].isMin)
            {
                foreach (var bus in buses)
                {
                    var position = bus.Contains(currentStartStop.Stop);
                    if (position > -1)
                    {
                        var cycleMultiplier = 0;
                        if (bus[position].When >= currentStartStop.When)
                        {
                            cycleMultiplier = 0;
                        }
                        else
                        {
                            cycleMultiplier = (currentStartStop.When - bus[position].When + bus.stopCycle - 1) / bus.stopCycle;
                        }
                        for (Int32 i = 1; i < bus.Length; i++)
                        {
                            var ind = (position + i);
                            var temp = bus[ind % bus.Length];
                            if (minTimes[temp.Stop].isMin) { continue; }
                            else
                            {
                                if (temp.When + (cycleMultiplier + ind / bus.Length) * bus.stopCycle < minTimes[temp.Stop].currentMin)
                                {
                                    minTimes[temp.Stop].currentMin = temp.When + (cycleMultiplier + ind / bus.Length) * bus.stopCycle;
                                    minTimes[temp.Stop].route = new List<RouteMap>(minTimes[currentStartStop.Stop].route);
                                    minTimes[temp.Stop].route.Add(new RouteMap(currentStartStop.Stop + 1,
                                        bus.Number + 1, bus[position].When + cycleMultiplier * bus.stopCycle));
                                }
                            }
                        }
                    }
                    else continue;
                }
                var time = upperTime;
                var index = -1;
                for (int i = 0; i < minTimes.Length; i++)
                {
                    if (!minTimes[i].isMin && minTimes[i].currentMin < time)
                    {
                        index = i;
                        time = minTimes[i].currentMin;
                    }
                }
                if (index != -1)
                {
                    currentStartStop = new StopTime(index, time);
                    minTimes[index].isMin = true;
                }
            }
        }
        public static void FindMinCost(Int32 start, Int32 finish, Int32 startTime) {
            StopCost currentStartStop = new StopCost(start-1, 0, startTime);
            while (!minCosts[finish-1].isMin)
            {
                foreach (var bus in buses)
                {
                    var position = bus.Contains(currentStartStop.Stop);
                    if (position > -1)
                    {
                        var cycleMultiplier = 0;
                        if (bus[position].When >= currentStartStop.Time)
                        {
                            cycleMultiplier = 0;
                        }
                        else
                        {
                            cycleMultiplier = (currentStartStop.Time - bus[position].When + bus.stopCycle - 1) / bus.stopCycle;
                        }
                        for (Int32 i = 1; i < bus.Length; i++)
                        {
                            var ind = (position + i);
                            var temp = bus[(position + i) % bus.Length];
                            if (minCosts[temp.Stop].isMin) { continue; }
                            else
                            {
                                if (minCosts[currentStartStop.Stop].currentMin + bus.Cost < minCosts[temp.Stop].currentMin &&
                                    (cycleMultiplier + ind / bus.Length) * bus.stopCycle < upperTime)
                                {
                                    minCosts[temp.Stop].time = temp.When + (cycleMultiplier + ind / bus.Length) * bus.stopCycle;
                                    minCosts[temp.Stop].currentMin = minCosts[currentStartStop.Stop].currentMin + bus.Cost;
                                    minCosts[temp.Stop].route = new List<RouteMap>(minCosts[currentStartStop.Stop].route);
                                    minCosts[temp.Stop].route.Add(new RouteMap(currentStartStop.Stop + 1,
                                        bus.Number + 1, bus[position].When + cycleMultiplier * bus.stopCycle));
                                }
                            }
                        }
                    }
                    else continue;
                }
                var cost = Int32.MaxValue;
                var index = -1;
                for (int i = 0; i < minCosts.Length; i++)
                {
                    if (!minCosts[i].isMin && minCosts[i].currentMin < cost)
                    {
                        index = i;
                        cost = minCosts[i].currentMin;
                    }
                }
                if (index != -1)
                {
                    currentStartStop = new StopCost(index, cost, minCosts[index].time);
                    minCosts[index].isMin = true;
                }
            }
        }
        static void Main(string[] args)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory(), "task.txt");
            ReadData(path);

            minTimes[start-1].isMin = true;
            minTimes[start - 1].currentMin = startTime;
            FindMinTime(start, finish, startTime);

            minCosts[start - 1].isMin = true;
            minCosts[start - 1].currentMin = 0;
            minCosts[start - 1].time = startTime;
            FindMinCost(start, finish, startTime);

            foreach (var a in minTimes[finish - 1].route) {
                Console.WriteLine(a.ToString());
            }
            Console.WriteLine("Время прибытия - {0}:{1:D2}", 
                minTimes[finish - 1].currentMin / 60, minTimes[finish - 1].currentMin % 60);
            
            foreach (var a in minCosts[finish - 1].route)
            {
                Console.WriteLine(a.ToString());
            }
            Console.WriteLine("Cуммарная стоимость поездки - {0}",
                minCosts[finish - 1].currentMin);

            Console.ReadLine();
        }
    }
}
