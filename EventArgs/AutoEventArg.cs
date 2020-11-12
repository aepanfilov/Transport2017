using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Транспорт2017
{
    public class AutoEventArg : EventArgs
    {
        //public Авто авто;
        public int кодОст, текТакт;
        public AutoEventArg(/*Авто авто,*/ int кодОст, int текТакт)
        {
            //me.авто = авто;
            this.кодОст = кодОст;
            this.текТакт = текТакт;
        }
    }
}
