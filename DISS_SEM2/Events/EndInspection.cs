using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class EndInspection : eventSTK
    {
        public EndInspection(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) 
            : base(_core, _time, _customer, _technician, _automechanic)
        {
        }
        public void execute()
        { 
            //nieje busy
            //preparkuje auto na parking lot
            //skontroluje rad v dielni ak je tam nieco naplanuje novu inspection

        }
    }
}
