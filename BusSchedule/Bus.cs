using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusSchedule
{
    public class StopTime
    {
        public Int32 Stop { get; set; }
        public Int32 When { get; set; }
        public StopTime(Int32 s, Int32 t)
        {
            Stop = s;
            When = t;
        }
    }
    public class Bus
    {
        private Int32 _number;
        private Int32 _departureTime;
        private Int32 _cost;

        public Int32 stopCycle = 0;
        public Int32 length;
        public Int32 Number { get { return _number; } }
        public Int32 DepartureTime { 
            get { return _departureTime; }
            set { _departureTime = value; }
        }
        public Int32 Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }
        public StopTime[] route;
        public StopTime this[Int32 position] {
            get {
                if (position >= length)
                    throw new ArgumentOutOfRangeException();
                return route[position];
            }
            set {
                if (position >= length)
                    throw new ArgumentOutOfRangeException();
                route[position] = value;
            }
        }
        public Bus(Int32 number) {
            _number = number;
        }
        public void RouteInitialization(Int32 n) {
            length = n;
            route = new StopTime[n];
        }
        public void DepartureTimeInitialization(Int32 hours, Int32 minutes) {
            _departureTime = hours * 60 + minutes;
        }
        
        public Int32 Contains(Int32 stop) {
            var left = 0;
            var right = length;
            if (left == right)
                return left;
            while (true)
            {
                if (right - left == 1)
                {
                    if (route[left].Stop == stop)
                        return left;
                    if (route[right].Stop == stop)
                        return right;
                    return -1;
                }
                else
                {
                    var middle = left + (right - left) / 2;
                    if (route[middle].Stop == stop)
                        return middle;
                    if (route[middle].Stop < stop)
                        left = middle;
                    if (route[middle].Stop > stop)
                        right = middle;
                }
            }
        }
    }
}
