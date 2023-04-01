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
            
            automechanic.obsluhuje = false;
            automechanic.customer_car = null;

            //pozriet do garaze
            //ak je tam auto naplanujem sam seba

            var newAutomechanic = ((STK)core).getAvailableAutomechanic();

            if (newAutomechanic != null) 
            {
                if (((STK)core).getCarsCountInGarage() > 0 )
                {
                    var customerFromGarage = ((STK)core).getNextCarInGarage();
                    var newInspection = new StartInspection(core, time, customerFromGarage, null, newAutomechanic);
                    core.AddEvent(newInspection);
                    newAutomechanic.obsluhuje = true;
                    newAutomechanic.customer_car = customerFromGarage;
                    ((STK)core).removeCarFromGarage();
                }
            }
            
















/*

            //nieje busy
            automechanic.obsluhuje = false;
            //preparkuje auto na parking lot
            ((STK)core).parkCarInParkingLot(automechanic.customer_car);
            //zakaznik prejde do payment line
            ((STK)core).addCustomerToPaymentLine(automechanic.customer_car);// same as customer
            automechanic.customer_car = null;
            //moze vyvolat platbu ?
            if (((STK)core).getFreeTechnicianCount() > 0)
            {
                var technic = ((STK)core).getAvailableTechnician();
                if (technic != null)
                {
                    technic.obsluhuje = true;
                    var nextCustomer = ((STK)core).getCustomerInPaymentLine();
                    var nextPaymentTime = ((STK)core).paymentTimeGenerator.Next() + time;

                    var newPayment = new Payment(core, nextPaymentTime, nextCustomer, technic, null);
                    core.AddEvent(newPayment);
                }
            }

            //skontroluje rad v dielni ak je tam nieco naplanuje novu inspection

            if (((STK)core).getCarsCountInGarage() > 0)
            {
                //naplanujem prevzatie auta automechanikom (musim zacat od list[0]), 
                var newAutomechanic = ((STK)core).getAvailableAutomechanic();
                var nextCustomer = ((STK)core).getNextCarInGarage();
                ((STK)core).removeCarFromGarage();

                if (newAutomechanic != null && nextCustomer != null)
                {
                    newAutomechanic.customer_car = nextCustomer;
                    newAutomechanic.obsluhuje = true;
                    var startInspection = new StartInspection(core, time, nextCustomer , null, newAutomechanic);
                    core.AddEvent(startInspection);
                }
            }*/

            

        }
    }
}
