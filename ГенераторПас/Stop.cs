﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Транспорт2017.ГенераторПас
{
    class Stop
    {
        public int CodeStop { get; set; }
        public string NameStop { get; set; }
        public string District { get; set; }
        public int CountPass { get; set; }//кол пассажиров, отправляющихся с данной остановки
        public int Attraction { get; set; }//привлекательность
        public double PercentageSitizen { get; set; } //доля жителей в окресности остановки
    }
}
