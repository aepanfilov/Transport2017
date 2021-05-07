using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Транспорт2017.ГенераторПас
{
    public class Passenger
    {
        public int CodeStopStart { get; set; }
        public DateTime Time { get; set; }
        public int CodeDistrictFinish { get; set; }
        public int CodeStopFinish { get; set; }
    }
}
