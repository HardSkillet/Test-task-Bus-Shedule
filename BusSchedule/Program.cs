using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule
{
    public static class Program
    {
        
        const Int32 upperTime = 1440;
        static Int32 N;
        static Int32 K;
        static MinTime[] minTime;
        static Bus[] buses;
        public class MinTime {
            public Boolean isMin = false;
            public Int32 currentMin = upperTime;
            public List<RouteMap> route = new List<RouteMap>();
        }
        static Int32 ReadData(String path) {
            var minFirstStopTime = upperTime;
            var index = -1;
            using (StreamReader fs = new StreamReader(path))
            {
                N = Int32.Parse(fs.ReadLine());
                K = Int32.Parse(fs.ReadLine());

                minTime = new MinTime[K];
                for (Int32 i = 0; i < K; i++) {
                    minTime[i] = new MinTime();
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
                    if (temp[1] == "1") {
                        if (buses[i].DepartureTime < minFirstStopTime) {
                            minFirstStopTime = buses[i].DepartureTime;
                            index = i;
                        }
                    }
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
            return index;
        }
        static void Main(string[] args)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory(), "task.txt");
            var busNumber = ReadData(path);
            minTime[0].isMin = true;
            minTime[0].currentMin = buses[busNumber].DepartureTime;

            StopTime currentStop = new StopTime(0, minTime[0].currentMin);
            
            while (!minTime[K-1].isMin) {
                foreach (var bus in buses) {
                    var position = bus.Contains(currentStop.Stop);
                    if (position > -1) {
                        var cycleMultiplier = 0;
                        if (bus[position].When >= currentStop.When) {
                            cycleMultiplier = 0;
                        } else {
                            cycleMultiplier = (currentStop.When - bus[position].When + bus.stopCycle - 1) / bus.stopCycle;
                            cycleMultiplier *= bus.stopCycle;
                        }
                        for (Int32 i = 0; i < bus.length; i++) {
                            var temp = bus[(position + i) % bus.length];
                            if (minTime[temp.Stop].isMin) { continue; }
                            else {
                                if (temp.When + cycleMultiplier < minTime[temp.Stop].currentMin) {
                                    minTime[temp.Stop].currentMin = temp.When + cycleMultiplier;
                                    minTime[temp.Stop].route = new List<RouteMap>(minTime[currentStop.Stop].route);
                                    minTime[temp.Stop].route.Add(new RouteMap(currentStop.Stop + 1, bus.Number + 1, bus[position].When + cycleMultiplier));
                                }
                            }
                        }
                    } else continue;
                }
                var time = upperTime;
                var index = -1;
                for (int i = 0; i < minTime.Length; i++) {
                    if (!minTime[i].isMin && minTime[i].currentMin < upperTime) {
                        index = i;
                        time = minTime[i].currentMin;
                    }
                }
                if (index != -1)
                {
                    currentStop = new StopTime(index, time);
                    minTime[index].isMin = true;
                }
            }
            foreach (var a in minTime[K - 1].route) {
                Console.WriteLine(a.ToString());
            }
            Console.WriteLine("Время прибытия - {0}:{1}", 
                minTime[K - 1].currentMin / 60, minTime[K - 1].currentMin % 60 == 0 ? "00" : (minTime[K - 1].currentMin % 60).ToString());
            Console.ReadLine();
        }
    }
}
