using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class SystemEvent : eventSTK
    {
        public SystemEvent(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) : base(_core, _time, _customer, _technician, _automechanic)
        {
        }
        public override void execute()
        {
            //time will be set according to desired speed
            Thread.Sleep(((STK)core).ReturnSpeed());
            var nextTime = 1 + time; // po sekunde
            core.AddEvent(new SystemEvent(core, nextTime, null, null, null));
        }
    }
}
