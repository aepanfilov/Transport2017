using System;

namespace Транспорт2017
{
    public class IntEventArg : EventArgs
    {
        public int value;
        public IntEventArg(int value)
        {
            this.value = value;
        }
    }
}