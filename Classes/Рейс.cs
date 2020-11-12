using System;

namespace Транспорт2017
{
    public class Рейс : IComparable<Рейс>
    {
        public int времяОтправления;
        public int МаксВместимость;

        public int CompareTo(Рейс other)
        {
            return времяОтправления.CompareTo(other.времяОтправления);
        }
    }
}