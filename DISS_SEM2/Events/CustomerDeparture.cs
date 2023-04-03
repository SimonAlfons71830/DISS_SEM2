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
        public override void execute()
        {
            //statistics
            ((STK)core).localAverageCustomerTimeInSTK.addValues(time - customer.arrivalTime);

            //uvolni technika
            //naplanuje novu platbu
            //ak nikto nieje v rade tak naplanuje start takeovber

            technician.obsluhuje = false;
            technician.customer_car = null;

            var technic = ((STK)core).getAvailableTechnician();

            if (technic != null) 
            {
                if (((STK)core).getCustomersCountInPaymentLine() > 0) 
                {
                    //platba
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();
                    technic.obsluhuje = true;
                    var newPayment = new Payment(core, paymentTime, paymentCustomer, technic, null);
                    core.AddEvent(newPayment);
                    ((STK)core).removeCustomerFromPaymentLine();
                }
                else
                {
                    if (((STK)core).getCustomersCountInLine() > 0)
                    {
                        if (((STK)core).reserveParking())
                        {
                            var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                            var takeoverCustomer = ((STK)core).getCustomerInLine();

                            technic.obsluhuje = true;
                            technic.customer_car = takeoverCustomer;
                            var newTakeover = new StartTakeOver(core, takeoverTime, takeoverCustomer, technic, null);
                            core.AddEvent(newTakeover);
                            ((STK)core).removeCustomerFromLine();
                        }
                    }
                }
            }
            


        }
    }
}
