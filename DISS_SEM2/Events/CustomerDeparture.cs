using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class CustomerDeparture : eventSTK
    {
        public CustomerDeparture(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) : 
            base(_core, _time, _customer, _technician, _automechanic)
        {
        }
        public void execute()
        {
            //vezme si auto spred garaze a odchadza
            if (((STK)core).getCustomersCarFromParkingLot(customer.getCar()))
            {
                //leaving
            } 
        }
    }
}
