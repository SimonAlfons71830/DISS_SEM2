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
            var pom = time - customer.arrivalTime; 
            
            ((STK)core).localAverageCustomerTimeInSTK.addValues(time - customer.arrivalTime);
            //uvolni technika
            //naplanuje novu platbu
            //ak nikto nieje v rade tak naplanuje start takeovber

            //predtym ako ho pridam
            var _timeTechnicBack = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
            ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnicBack);
            ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

            technician.obsluhuje = false;
            //pridali sme ho do volnych 



            technician.customer_car = null;

            /*if (((STK)core).getAvailableTechnicianCount() > 0)
            {
                //predtym ako ho vyberiem
                var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);
            }*/


            var technic = ((STK)core).getAvailableTechnician();

            if (technic != null) 
            {
                

                if (((STK)core).getCustomersCountInPaymentLine() > 0) 
                {
                    //platba
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();

                    var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                    ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                    ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

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

                            //stat III
                            var _time = core.currentTime - ((STK)core).localAverageCustomerCountInLineToTakeOver.timeOfLastChange;
                            ((STK)core).localAverageCustomerCountInLineToTakeOver.addValues(((STK)core).getCustomersCountInLine(), _time);
                            ((STK)core).localAverageCustomerCountInLineToTakeOver.setFinalTimeOfLastChange(core.currentTime);

                            ((STK)core).removeCustomerFromLine();
                        }
                    }
                }
            }


            ((STK)core).localAverageCustomerCountInSTK.addValues(((STK)core).customerscount, core.currentTime - ((STK)core).localAverageCustomerCountInSTK.timeOfLastChange);
            ((STK)core).localAverageCustomerCountInSTK.setFinalTimeOfLastChange(core.currentTime);

            ((STK)core).customerscount--;
            

        }
    }
}
