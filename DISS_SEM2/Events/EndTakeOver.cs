﻿using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void execute()
        {
            technician.obsluhuje = false;

            if (((STK)core).getCustomersCountInPaymentLine() == 0)
            {
                if (((STK)core).getCustomersCountInLine() != 0 &&
                ((STK)core).getTechniciansCount() != 0)
                {
                    var technic = ((STK)core).getAvailableTechnician();

                    if (technic != null && ((STK)core).getFreeSpacesInGarage() < 5)
                    {
                        var newCustomer = ((STK)core).getCustomerInLine();

                        var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + 1;
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

                    var payment = new Payment(core,paymentTime,paymentCustomer,technic,null); //nastavit ho ze obsluhuje
                    core.AddEvent(payment);

                    ((STK)core).removeCustomerFromPaymentLine(paymentCustomer);
                }
            }

            if (((STK)core).getCarsCountInGarage() > 0)
            {
                //naplanujem prevzatie auta automechanikom (musim zacat od list[0]), 
                var newAutomechanic = ((STK)core).getAvailableAutomechanic();
                var nextCar = ((STK)core).getNextCarInGarage();
                ((STK)core).removeCarFromGarage(nextCar);

                if (newAutomechanic != null && nextCar != null)
                {
                    newAutomechanic.car = nextCar;
                    var startInspection = new StartInspection(core, time, customer, null, newAutomechanic);
                    core.AddEvent(startInspection);
                }
            }
            
        }
    }
}
