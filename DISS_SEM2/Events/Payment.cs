using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class Payment : eventSTK
    {
        public Payment(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) 
            : base(_core, _time, _customer, _technician, _automechanic)
        {
        }

        public override void execute()
        {
            //musim najst auto svoje v parking lot
            //naplanuje departuje - uvolni technika
            //zakaznik je uz vymazany v endtakeover

            ((STK)core).removeCarFromParkingLot(customer);
            var newDeparture = new CustomerDeparture(core, time, customer, technician, null); //este ho posielam a potom vymazem
            core.AddEvent(newDeparture);


        }
    }
}
