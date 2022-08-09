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
            }
        }
        public static void FindMinTime() {
            StopTime currentStop = new StopTime(0, minTimes[0].currentMin);

            while (!minTimes[K - 1].isMin)
            {
                foreach (var bus in buses)
                {
                    var position = bus.Contains(currentStop.Stop);
                    if (position > -1)
                    {
                        var cycleMultiplier = 0;
                        if (bus[position].When >= currentStop.When)
                        {
                            cycleMultiplier = 0;
                        }
                        else
                        {
                            cycleMultiplier = (currentStop.When - bus[position].When + bus.stopCycle - 1) / bus.stopCycle;
                            cycleMultiplier *= bus.stopCycle;
                        }
                        for (Int32 i = 0; i < bus.Length; i++)
                        {
                            var temp = bus[(position + i) % bus.Length];
                            if (minTimes[temp.Stop].isMin) { continue; }
                            else
                            {
                                if (temp.When + cycleMultiplier < minTimes[temp.Stop].currentMin)
                                {
                                    minTimes[temp.Stop].currentMin = temp.When + cycleMultiplier;
                                    minTimes[temp.Stop].route = new List<RouteMap>(minTimes[currentStop.Stop].route);
                                    minTimes[temp.Stop].route.Add(new RouteMap(currentStop.Stop + 1, bus.Number + 1, bus[position].When + cycleMultiplier));
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
                    currentStop = new StopTime(index, time);
                    minTimes[index].isMin = true;
                }
            }
        }
        public static void FindMinCost() {
            StopCost currentStop = new StopCost(0, 0, 0);
            while (!minCosts[K - 1].isMin)
            {
                foreach (var bus in buses)
                {
                    var position = bus.Contains(currentStop.Stop);
                    if (position > -1)
                    {
                        var cycleMultiplier = 0;
                        if (bus[position].When >= currentStop.Time)
                        {
                            cycleMultiplier = 0;
                        }
                        else
                        {
                            cycleMultiplier = ((currentStop.Time - bus[position].When + bus.stopCycle - 1) / bus.stopCycle) * bus.stopCycle;
                        }
                        for (Int32 i = 0; i < bus.Length; i++)
                        {
                            var temp = bus[(position + i) % bus.Length];
                            if (minCosts[temp.Stop].isMin) { continue; }
                            else
                            {
                                if (minCosts[currentStop.Stop].currentMin + bus.Cost < minCosts[temp.Stop].currentMin &&
                                    temp.When + cycleMultiplier < upperTime)
                                {
                                    minCosts[temp.Stop].time = temp.When + cycleMultiplier;
                                    minCosts[temp.Stop].currentMin = minCosts[currentStop.Stop].currentMin + bus.Cost;
                                    minCosts[temp.Stop].route = new List<RouteMap>(minCosts[currentStop.Stop].route);
                                    minCosts[temp.Stop].route.Add(new RouteMap(currentStop.Stop + 1, bus.Number + 1, bus[position].When + cycleMultiplier));
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
                    currentStop = new StopCost(index, cost, minCosts[index].time);
                    minCosts[index].isMin = true;
                }
            }
        }
        static void Main(string[] args)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory(), "task.txt");
            ReadData(path);

            minTimes[0].isMin = true;
            minTimes[0].currentMin = 0;
            FindMinTime();

            minCosts[0].isMin = true;
            minCosts[0].currentMin = 0;
            minCosts[0].time = 0;
            FindMinCost();

            foreach (var a in minTimes[K - 1].route) {
                Console.WriteLine(a.ToString());
            }
            Console.WriteLine("Время прибытия - {0}:{1}", 
                minTimes[K - 1].currentMin / 60, minTimes[K - 1].currentMin % 60 == 0 ? "00" : (minTimes[K - 1].currentMin % 60).ToString());
            
            foreach (var a in minCosts[K - 1].route)
            {
                Console.WriteLine(a.ToString());
            }
            Console.WriteLine("Cуммарная стоимость поездки - {0}",
                minCosts[K-1].currentMin);

            Console.ReadLine();
        }
    }
}
