using System;

namespace Транспорт2017
{
    public struct Перегон:IComparable<Перегон>
    {
        public int codeOst;
        public double length;
        public int timeInt;
        public int number;

        public int CompareTo(Перегон other)
        {
            return number.CompareTo(other.number);
        }
    }
}
