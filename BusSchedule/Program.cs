using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule
{
    class Program
    {
        const Int32 upperTime = 1440;
        public class MinTime {
            public Boolean isMin = false;
            public Int32 currentMin = upperTime;
            public List<Int32> route = new List<Int32>();
        }
        static void Main(string[] args)
        {
            MinTime[] minTime = new MinTime[4];
            Boolean[] isMinNow = new Boolean[3];

            Int32 n = 0;
            Int32 k = 0;
            Bus[] buses = new Bus[3];
            for (Int32 i = 0; i < n; i++) {
                buses[i] = new Bus(i);              //depTime
            }
            for (Int32 i = 0; i < n; i++)
            {
                buses[i].Cost = 312;
            }
            for (Int32 i = 0; i < n; i++)
            {
                var countStop = 3;
                buses[i].RouteInitialization(countStop);
                for (Int32 j = 0; j < countStop; j++) {
                    buses[i][j] = new StopTime(1, 0);
                }
                for (Int32 j = 0; j < countStop; j++)
                {
                    buses[i][j].When = buses[i].DepartureTime + buses[i].stopCycle;
                    //read
                    buses[i].stopCycle += 12312;
                }
            }



            List<StopTime> queue = new List<StopTime>();
            queue.Add(new StopTime(1, 60*12));
            
            minTime[0].isMin = true;
            minTime[0].route.Add(1);
            minTime[0].currentMin = 12 * 60;
            
            while (queue.Count != 0) {
                var currentStop = queue[0].Stop;
                foreach (var b in buses) {
                    var position = b.Contains(currentStop);
                    if (position > -1) {

                        var cycleMultiplier = (Int32)0;
                        if (b[position].When >= queue[0].When) {
                            cycleMultiplier = 0;
                        } else {
                            cycleMultiplier = (queue[0].When - b[position].When + b.stopCycle - 1) / b.stopCycle;
                            cycleMultiplier *= b.stopCycle;
                        }
                        for (Int32 i = 0; i < b.length; i++) {
                            var temp = b[(position + i) % b.length];
                            if (minTime[temp.Stop].isMin) { continue; }
                            else {
                                if (temp.When + cycleMultiplier < minTime[temp.Stop].currentMin) {
                                    minTime[temp.Stop].currentMin = temp.When + cycleMultiplier;
                                    minTime[temp.Stop].route = new List<Int32>(minTime[queue[0].Stop].route);
                                    minTime[temp.Stop].route.Add(temp.Stop);
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
                    ; ; ;
                    minTime[index].isMin = true;
                }


            }
        }
    }
}
