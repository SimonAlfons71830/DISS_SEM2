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

            //preparkovat auto
            //zrusit obsluhu
            //zakaznik sa presunie do radu

            ((STK)core).parkCarInParkingLot(automechanic.customer_car);
            ((STK)core).addCustomerToPaymentLine(automechanic.customer_car);

            //viem ze sa niekto zaradil do radu na platenie, mozem dat robotu technikovi
            //stat musim robit predtym ako ho vyberiem z listu
            


            var technic = ((STK)core).getAvailableTechnician();
            if (technic != null) 
            {
                var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

                //payment
                var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                var payingCustomer = ((STK)core).getCustomerInPaymentLine();

                technic.obsluhuje = true;
                technic.customer_car = payingCustomer;
                //!!!!!
                var newPayment = new Payment(core, paymentTime, payingCustomer, technic,null);
                
                

                core.AddEvent(newPayment);
                ((STK)core).removeCustomerFromPaymentLine();
            }



            //predtym ako ho vratim do free obsluhovania
             var _timeAutomechanicBack = core.currentTime - ((STK)core).localAverageFreeAutomechanicCount.timeOfLastChange;
             ((STK)core).localAverageFreeAutomechanicCount.addValues(((STK)core).getAvailableAutomechanicCount(), _timeAutomechanicBack);
             ((STK)core).localAverageFreeAutomechanicCount.setFinalTimeOfLastChange(core.currentTime);
            

            automechanic.obsluhuje = false;
            automechanic.customer_car = null;

            //pozriet do garaze
            //ak je tam auto naplanujem sam seba


            var newAutomechanic = ((STK)core).getAvailableAutomechanic();

            if (newAutomechanic != null) 
            {
                if (((STK)core).getCarsCountInGarage() > 0 )
                {
                    var _timeAutomechanicGet = core.currentTime - ((STK)core).localAverageFreeAutomechanicCount.timeOfLastChange;
                    ((STK)core).localAverageFreeAutomechanicCount.addValues(((STK)core).getAvailableAutomechanicCount(), _timeAutomechanicGet);
                    ((STK)core).localAverageFreeAutomechanicCount.setFinalTimeOfLastChange(core.currentTime);

                    var customerFromGarage = ((STK)core).getNextCarInGarage();
                    var newInspection = new StartInspection(core, time, customerFromGarage, null, newAutomechanic);
                    core.AddEvent(newInspection);

                    newAutomechanic.obsluhuje = true;
                    newAutomechanic.customer_car = customerFromGarage;

                    ((STK)core).removeCarFromGarage();
                }
            }
            
        }
    }
}
