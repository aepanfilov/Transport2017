using System.Collections.Generic;

namespace Транспорт2017
{
    class ПассажирыПоМаршрутам
    {
        public int sumPas;//сумма пас желающих перевезтись
        public Dictionary<int, int> dict;//ключ = код маршрута, значение = число перевезенных

        public ПассажирыПоМаршрутам()
        {
            dict = new Dictionary<int, int>();
        }
    }
}