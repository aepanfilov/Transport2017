using System;
using System.Collections.Generic;

namespace Транспорт2017.ГенераторПас
{
    internal class District
    {
        public int CodeDistrict { get; set; }
        public string NameDistrict { get; set; }
        public List<Stop> ListStop { get; set; }
        public int CountStops { get { return ListStop.Count; } }

        public District()
        {
            ListStop = new List<Stop>();
        }
        internal Stop GetStop(int j_stops)
        {
            return ListStop[j_stops];
        }
    }
}