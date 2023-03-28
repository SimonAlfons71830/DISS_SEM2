using DISS_SEM2.Core;
using DISS_SEM2.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class CustomerArrival : eventSTK
    {
        public CustomerArrival(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) 
            : base(_core, _time, _customer, _technician, _automechanic)
        {
        }

        public override void execute()
        {

            var nextArrivalTime = ((STK)core).customerArrivalTimeGenerator.Next() + time;
            core.AddEvent(new CustomerArrival(core, nextArrivalTime, new Customer(nextArrivalTime, new Car(((STK)core).carTypeGenerator.Next())),null,null));


            if (((STK)core).getCustomersCountInPaymentLine() == 0 && ((STK)core).getTechniciansCount() != 0)
            {
                //prebratie auta trva dlhsie a ludia chodia hned, tym padom 3ja dostanu toho isteho technika naraz
                //rovno ked najdem available tak ho nastavim na obsluhuje ak sa vytvara event 
                var technic = ((STK)core).getAvailableTechnician();
                if (technic != null && ((STK)core).getFreeSpacesInGarage() < 5)
                {
                    //trojuholnikove rozdelenie na prijatie auta a preparkovanie do garaze
                    var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                    technic.obsluhuje = true;
                    var startTakeOver = new StartTakeOver(core, takeoverTime, customer, technic, null);
                    core.AddEvent(startTakeOver);
                }
                else
                {
                    ((STK)core).addCustomerToLine(customer);
                }
            }
            else
            {
                ((STK)core).addCustomerToLine(customer);
            }
            


        }
    }
}
