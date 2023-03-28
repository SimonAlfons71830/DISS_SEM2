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
            //technician.obsluhuje = true;
            var payingCustomer = ((STK)core).getCustomerInPaymentLine(); //uz je osetrene ci tam niekto je
            ((STK)core).removeCustomerFromPaymentLine(payingCustomer);
            //zaplatil naplanuje event odchod
            technician.obsluhuje = false;

            var customerDeparture = new CustomerDeparture(core,time,customer,null,null);
            core.AddEvent(customerDeparture);

        }
    }
}
