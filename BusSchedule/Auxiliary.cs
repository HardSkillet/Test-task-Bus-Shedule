using System;

namespace BusSchedule
{
    public class StopCost
    {
        public Int32 Stop { get; set; }
        public Int32 Cost { get; set; }
        public Int32 Time { get; set; }
        public StopCost(Int32 s, Int32 c, Int32 t)
        {
            Stop = s;
            Cost = c;
            Time = t;
        }
    }
    public class StopTime
    {
        public Int32 Stop { get; set; }
        public Int32 When { get; set; }
        public StopTime(Int32 s) : this(s, 0)
        { }
        public StopTime(Int32 s, Int32 t)
        {
            Stop = s;
            When = t;
        }
    }
}
