using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Транспорт2017.ГенераторПас
{
    class Stop: IEquatable<Stop>
    {
        public int CodeStop { get; set; }
        public string NameStop { get; set; }
        public string District { get; set; }
        public int CountPass { get; set; } //кол пассажиров, отправляющихся с данной остановки
        public int Attraction { get; set; } //привлекательность
        public List<int> ListOfRoutes { get; set; } //количество маршрутов, проходящих через остановку
        public double PercentageSitizen { get; set; } //доля жителей в окресности остановки
        public List<Stop> StopWoTransfer { get; set; }
        public List<Stop> StopWithTransfer { get; set; }

        public bool Equals(Stop other)
        {
            return CodeStop.Equals(other.CodeStop);
        }

        public override string ToString()
        {
            return $"{CodeStop} {NameStop} {District}";
        }
    }
}
