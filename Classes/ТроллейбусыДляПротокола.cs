using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Транспорт2017
{
    /// <summary>
    /// Строка для протокола с троллейбусами
    /// </summary>
    public class ТроллейбусыДляПротокола
    {
        public int[] IntervalsTroll { get; set; }
        public int[] CountTCTroll { get; set; }
        public int[] AllTrollKm { get; set; }
        public int[] PassTroll { get; set; }
        public int AllTrollPass{ get; set; }
        public int AllTrollProfit{ get; set; }
    }
}
