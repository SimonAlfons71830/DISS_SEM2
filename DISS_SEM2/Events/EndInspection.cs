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
        public override void execute()
        {
            //nieje busy
            automechanic.obsluhuje = false;
            //preparkuje auto na parking lot
            ((STK)core).parkCarInParkingLot(automechanic.customer_car);
            //zakaznik prejde do payment line
            ((STK)core).addCustomerToPaymentLine(automechanic.customer_car);// same as customer
            automechanic.customer_car = null;

            //skontroluje rad v dielni ak je tam nieco naplanuje novu inspection


            if (((STK)core).getCarsCountInGarage() > 0)
            {
                //naplanujem prevzatie auta automechanikom (musim zacat od list[0]), 
                var newAutomechanic = ((STK)core).getAvailableAutomechanic();
                var nextCustomer = ((STK)core).getNextCarInGarage();
                ((STK)core).removeCarFromGarage(nextCustomer);

                if (newAutomechanic != null && nextCustomer != null)
                {
                    newAutomechanic.customer_car = nextCustomer;
                    newAutomechanic.obsluhuje = true;
                    var startInspection = new StartInspection(core, time, nextCustomer , null, newAutomechanic);
                    core.AddEvent(startInspection);
                }
            }

            

        }
    }
}
