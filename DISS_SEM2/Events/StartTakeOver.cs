using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class StartTakeOver : eventSTK
    {
        public StartTakeOver(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) 
            : base(_core, _time, _customer, _technician, _automechanic)
        {
        }

        public override void execute()
        {
            //uz obsluhuje z predtym aby bol obsadeny
            //technician.obsluhuje = true;
            technician.customer_car = customer;
            ((STK)core).parkCarInGarage(technician.customer_car);
            technician.customer_car = null;
            ((STK)core).removeCustomerFromLine(customer);

            var endTakeOver = new EndTakeOver(core, time, customer,technician,null);
            endTakeOver.execute();
            //zavolat koniec prijatia(+ technik) - neobslujuje 

            
        }
    }
}
