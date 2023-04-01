using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class EndTakeOver : eventSTK
    {
        public EndTakeOver(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) 
            : base(_core, _time, _customer, _technician, _automechanic)
        {
        }

        public override void execute()
        {
            //vymazem auto technikovi
            //nastavim mu obsluhu na false
            //
            //volny automechanic - je vyvolam inspection tak ze beriem z garaze nie z tejto triedy

            technician.customer_car = null;
            technician.obsluhuje = false;

            var newAutomechanic = ((STK)core).getAvailableAutomechanic();
            //null alebo je free

            if (newAutomechanic != null)
            {
                var customerFromGarage = ((STK)core).getNextCarInGarage();
                newAutomechanic.obsluhuje = true;
                newAutomechanic.customer_car = customerFromGarage;
                var startInsepction = new StartInspection(core, time, customerFromGarage, null, newAutomechanic);
                core.AddEvent(startInsepction);
                ((STK)core).removeCarFromGarage();
            }

            //startTakeover alebo payment podla toho ci je
            //free technic potom
            //ci je v rade na payment niekto a potom
            //ci je volne miesto na park

            var technic = ((STK)core).getAvailableTechnician(); //bude volny 
            if (technic != null) 
            {
                if (((STK)core).getCustomersCountInPaymentLine() > 0)
                {

                    //payment
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();
                    technic.obsluhuje = true;
                    var newPayment = new Payment(core, paymentTime, paymentCustomer, technic, null);
                    core.AddEvent(newPayment);
                    ((STK)core).removeCustomerFromPaymentLine();
                }
                else
                {
                    if (((STK)core).getCarsCountInGarage() < 5)
                    {
                        if (((STK)core).getCustomersCountInLine() > 0)
                        {
                            //takeover
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
            












            /*technician.obsluhuje = false;

            if (((STK)core).getCustomersCountInPaymentLine() == 0)
            {
                if (((STK)core).getCustomersCountInLine() > 0 &&
                ((STK)core).getTechniciansCount() > 0)
                {
                    var technic = ((STK)core).getAvailableTechnician();

                    if (technic != null && ((STK)core).getParkedCarsCountInGarage() < 5) //5 alebo 4?
                    {
                        var newCustomer = ((STK)core).getCustomerInLine();

                        var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                        var startTakeOver = new StartTakeOver(core, takeoverTime, newCustomer, technic, null);
                        core.AddEvent(startTakeOver);

                        ((STK)core).removeCustomerFromLine(customer);
                    }
                }
            }
            else
            {
                var technic = ((STK)core).getAvailableTechnician();
                if (technic != null)
                {
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    technic.obsluhuje = true;
                    var payment = new Payment(core,paymentTime,paymentCustomer,technic,null); //nastavit ho ze obsluhuje
                    core.AddEvent(payment);

                    ((STK)core).removeCustomerFromPaymentLine();
                }
            }

            if (((STK)core).getCarsCountInGarage() > 0)
            {
                //naplanujem prevzatie auta automechanikom (musim zacat od list[0]), 
                var newAutomechanic = ((STK)core).getAvailableAutomechanic();
                var nextCustomer_car = ((STK)core).getNextCarInGarage();
                ((STK)core).removeCarFromGarage(nextCustomer_car);

                if (newAutomechanic != null && nextCustomer_car!= null)
                {
                    newAutomechanic.customer_car = nextCustomer_car;
                    newAutomechanic.obsluhuje = true;
                    var startInspection = new StartInspection(core, time, nextCustomer_car, null, newAutomechanic);
                    core.AddEvent(startInspection);
                }
            }*/
            
        }
    }
}
