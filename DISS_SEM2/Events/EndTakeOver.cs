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

            
            //predtym ako ho pridam
            var _timeTechnicBack = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
            ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnicBack);
            ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

            technician.obsluhuje = false;
            technician.customer_car = null;


            var newAutomechanic = ((STK)core).getAvailableAutomechanic();
            //null alebo je free

            if (newAutomechanic != null)
            {
                var customerFromGarage = ((STK)core).getNextCarInGarage();

                var _timeAtumechanicGet = core.currentTime - ((STK)core).localAverageFreeAutomechanicCount.timeOfLastChange;
                ((STK)core).localAverageFreeAutomechanicCount.addValues(((STK)core).getAvailableAutomechanicCount(), _timeAtumechanicGet);
                ((STK)core).localAverageFreeAutomechanicCount.setFinalTimeOfLastChange(core.currentTime);

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

                    var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                    ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                    ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

                    technic.obsluhuje = true;
                    technic.customer_car = paymentCustomer;

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

                            var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                            ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                            ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

                            technic.obsluhuje = true;
                            technic.customer_car = takeoverCustomer;

                            //statistika ratam iba tym co prevezmu auto
                            var _takeovertimestat = time - takeoverCustomer.arrivalTime;
                            ((STK)core).localAverageTimeToTakeOverCar.addValues(_takeovertimestat);

                            var newTakeover = new StartTakeOver(core, takeoverTime, takeoverCustomer, technic, null);
                            core.AddEvent(newTakeover);

                            //stitistika
                            var _time = core.currentTime - ((STK)core).localAverageCustomerCountInLineToTakeOver.timeOfLastChange;
                            ((STK)core).localAverageCustomerCountInLineToTakeOver.addValues(((STK)core).getCustomersCountInLine(), _time);
                            ((STK)core).localAverageCustomerCountInLineToTakeOver.setFinalTimeOfLastChange(core.currentTime);

                            ((STK)core).removeCustomerFromLine();
                        }


                    }
                }
            }

        }
    }
}
