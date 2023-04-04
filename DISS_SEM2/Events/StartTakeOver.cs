using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

            /*//statistika ratam iba tym co prevezmu auto
            var _takeovertimestat = time - customer.arrivalTime;
            ((STK)core).localAverageTimeToTakeOverCar.addValues(_takeovertimestat);*/

            //prichod z customer arrival
            //technika mam 
            //auto mam
            //parkovacie miesto mam
            //priradim auto technikovi
            //zaparkujem do garaze
            //vyvolam end takeover - cas ten isty, technic ten isty, automechanik stale null

            technician.customer_car = customer;
            ((STK)core).parkCarInGarage(customer);
            var endTakeover = new EndTakeOver(core, time, customer, technician, null);
            core.AddEvent(endTakeover);




            //prichod z endtakeover
            







            
        }
    }
}
