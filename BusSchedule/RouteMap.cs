using System;

namespace BusSchedule
{
    public class RouteMap
    {
        private Int32 numberOfStop;
        private Int32 numberOfBus;
        private Int32 time;
        public RouteMap(Int32 stop, Int32 bus, Int32 time)
        {
            numberOfStop = stop;
            numberOfBus = bus;
            this.time = time;
        }
        public override String ToString()
        {
            return String.Format("С остановки №{0} на автобусе №{1} в {2}:{3:D2}",
                numberOfStop, numberOfBus, time / 60, time % 60);
        }
    }
}
