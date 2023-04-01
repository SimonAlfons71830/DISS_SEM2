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






            /*//technician.obsluhuje = true;
            if (((STK)core).getCustomersCountInPaymentLine() >0)
            {
                var payingCustomer = ((STK)core).getCustomerInPaymentLine(); //uz je osetrene ci tam niekto je
                ((STK)core).removeCustomerFromPaymentLine();
            }
            else
            {
                //ide priamo z end takeover ESTE OSETRIT!!!
            }
            
            
            //zaplatil naplanuje event odchod
            technician.obsluhuje = false;

            var customerDeparture = new CustomerDeparture(core,time,customer,null,null);
            core.AddEvent(customerDeparture);

            if (((STK)core).getCustomersCountInPaymentLine() > 0)
            {
                var technic = ((STK)core).getAvailableTechnician();
                if (technic != null)
                {
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    technic.obsluhuje = true;
                    var payment = new Payment(core, paymentTime, paymentCustomer, technic, null); //nastavit ho ze obsluhuje
                    core.AddEvent(payment);

                    ((STK)core).removeCustomerFromPaymentLine();
                }
            }
            else
            {
                //urobi novy takeover zakaznika z frontu

                var technic = ((STK)core).getAvailableTechnician();
                if (((STK)core).getCustomersCountInLine() > 0 && technic != null)
                {
                    var nextCustomer = ((STK)core).getCustomerInLine();
                    var nextTime = ((STK)core).takeOverTimeGenerator.Next() + time; ;
                    technic.obsluhuje = true;
                    var newTakeover = new StartTakeOver(core, nextTime, nextCustomer, technic, null);
                    core.AddEvent(newTakeover);
                    ((STK)core).removeCustomerFromLine();
                }

            }*/
            

        }
    }
}
