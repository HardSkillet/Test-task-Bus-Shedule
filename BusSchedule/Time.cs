using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusScheduletrytryu
{
    public struct Time : IComparable<Time>
    {
        private Int32 hours;
        private Int32 minutes;
        public Int32 Hours { get { return hours; } }
        public Int32 Minutes { get { return Minutes; } }
        public Time(Int32 m)
        {
            hours = m / 60;
            minutes = m % 60;
        }
        public Time(Int32 h, Int32 m) {
            hours = h;
            minutes = m;
        }
        public Int32 CompareTo(Time t)
        {
            if (this.hours < t.hours)
            {
                return -1;
            }
            else if (this.hours > t.hours) { return 1; }
            else if (this.minutes < t.minutes) { return -1; }
            else if (this.minutes > t.minutes) { return 1; }
            else return 0;
        }
        public static Time operator +(Time t1, Time t2)
        {
            var m = t1.minutes + t2.minutes;
            var h = t1.hours + t2.hours + m / 60;
            if (h >= 24)
            {
                return new Time(-1, -1);
            }
            else {
                return new Time(h, m % 60);
            }
        }
        public static Boolean operator <(Time t1, Time t2) {
            if (t1.CompareTo(t2) == -1)
            {
                return true;
            }
            else return false;
        }
        public static Boolean operator >(Time t1, Time t2) {
            if (t1.CompareTo(t2) == 1)
            {
                return true;
            }
            else return false;
        }
    }
}
