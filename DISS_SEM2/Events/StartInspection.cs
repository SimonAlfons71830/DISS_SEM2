using DISS_SEM2.Core;
using DISS_SEM2.Objects.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class StartInspection : eventSTK
    {
        public StartInspection(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) : base(_core, _time, _customer, _technician, _automechanic)
        {
        }
        public override void execute()
        {
            //z end takeover
            //automechanik je obsadeny
            //cas inspection
            //naplanovat end
            automechanic.customer_car = customer;
            
            
            ((STK)core).freeParking(); //bez referencie - iba counter 
            double endsinspectionTime;
            if (customer.getCar().type == CarTypes.Personal)
            {
                endsinspectionTime = ((STK)core).personalCarInspectionGenerator.Next() + time;
            }
            else if (customer.getCar().type == CarTypes.Van)
            {
                endsinspectionTime = ((STK)core).vanCarInspectionGenerator.Next() + time;
            }
            else
            {
                endsinspectionTime = ((STK)core).cargoCarInspectionGenerator.Next() + time; 
            }
            var endInspection = new EndInspection(core, endsinspectionTime,customer,null,automechanic);
            core.AddEvent(endInspection);


            //uvolni sa miesto v garazi tak mozem automaticky vyvolat novy takeover ak nikto necaka v rade na platenie
            

            //ak je volny technik tak nikto necaka na platbu
            var technic = ((STK)core).getAvailableTechnician();
            if (technic != null)
            {

                
                /*if (((STK)core).getCustomersCountInPaymentLine() > 0)
                {
                    //payment
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    var payingCustomer = ((STK)core).getCustomerInPaymentLine();
                    var newPayment = new Payment(core, paymentTime, payingCustomer, technic, null);
                    core.AddEvent(newPayment);
                    ((STK)core).removeCustomerFromPaymentLine();
                }
                else*/
                if (((STK)core).getCustomersCountInLine() > 0) 
                {
                    if (((STK)core).reserveParking())
                    {
                        
                        //takeover
                        var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                        var takeoverCustomer = ((STK)core).getCustomerInLine();


                        //statistika ratam iba tym co prevezmu auto
                        var _takeovertimestat = time - takeoverCustomer.arrivalTime;
                        ((STK)core).localAverageTimeToTakeOverCar.addValues(_takeovertimestat);
                        var newTakeOver = new StartTakeOver(core, takeoverTime, takeoverCustomer, technic, null);

                        var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                        ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                        ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);

                        technic.obsluhuje = true;
                        core.AddEvent(newTakeOver);

                        //stat
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
