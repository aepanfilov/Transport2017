using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Транспорт2017.ГенераторПас
{
    public class Passenger
    {
        public int CodeStopStart { get; set; }
        public TimeSpan Time { get; set; }
        public int CodeDistrictFinish { get; set; }
        public int CodeStopFinish { get; set; }
        public override string ToString()
        {
            return $"ост {CodeStopStart}, р-н {CodeDistrictFinish}, назн {CodeStopFinish}";
        }
    }
}
