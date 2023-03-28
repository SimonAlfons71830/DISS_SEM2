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

        public void execute()
        {

            technician.obsluhuje = true;
            technician.car = customer.getCar();
            ((STK)core).parkCarInGarage(technician.car);
            technician.car = null;
            var endTakeOver = new EndTakeOver(core, time, customer,technician,null);
            endTakeOver.execute();
            //zavolat koniec prijatia(+ technik) - neobslujuje 
            
        }
    }
}
