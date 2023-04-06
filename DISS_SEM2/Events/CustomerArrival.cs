using DISS_SEM2.Core;
using DISS_SEM2.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace DISS_SEM2.Events
{
    internal class CustomerArrival : eventSTK
    {
        public CustomerArrival(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) 
            : base(_core, _time, _customer, _technician, _automechanic)
        {
        }

        public override void execute()
        {
            ((STK)core).todaysCustomers++;
            //prichod zakaznika vyvola dalsi prichod zakaznika - po 15 45 uz neplanujem
            //zistim freee technika
            //ak je free tak -> najprv paymentline 
            //ak nieje nikto v payment line tak kontrolujem dostupne parkovacie miesto
            // ak je free customerovi naplanuje takeover ak nieje free park miesto davam ho do rady
            // ak NEMAM free technika -> zakaznik ide do rady (prio je arrival time)

            ((STK)core).localAverageCustomerCountInSTK.addValues(((STK)core).customerscount);

            ((STK)core).customerscount++;

            if (time != core.currentTime)
            {
                return;
            }
            var nextTime = ((STK)core).customerArrivalTimeGenerator.Next() + time;
            if (nextTime <= 24300)
            {
                var nextCar = new Car(((STK)core).carTypeGenerator.Next());
                var nextCustomer = new Customer(nextTime, nextCar);
                ((STK)core).setId(nextCustomer);
                var nextArrival = new CustomerArrival(core, nextTime, nextCustomer, null, null);
                core.AddEvent(nextArrival);
            }

            

            var technic = ((STK)core).getAvailableTechnician();
            // bud je to technic ktory neobsluhuje alebo null

            if (technic != null)
            {


               /* if (((STK)core).getCustomersCountInPaymentLine() > 0)
                {
                    //tato situacia by nemala nikdy nastat
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();
                    ((STK)core).removeCustomerFromPaymentLine();
                    technic.obsluhuje = true;
                    technic.customer_car = paymentCustomer;
                    var nextPayment = new Payment(core, paymentTime, paymentCustomer, technic, null);
                    core.AddEvent(nextPayment);
                }
                else*/
                {
                    if (((STK)core).reserveParking())
                    {
                        var _timeTechnic = core.currentTime - ((STK)core).localAverageFreeTechnicianCount.timeOfLastChange;
                        ((STK)core).localAverageFreeTechnicianCount.addValues(((STK)core).getAvailableTechnicianCount(), _timeTechnic);
                        ((STK)core).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(core.currentTime);


                        var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                        technic.obsluhuje = true;
                        technic.customer_car = customer;

                        //statistika ratam iba tym co prevezmu auto
                        var _takeovertimestat = time - customer.arrivalTime;
                        ((STK)core).localAverageTimeToTakeOverCar.addValues(_takeovertimestat);

                        var newTakeover = new StartTakeOver(core, takeoverTime, customer, technic, null);

                        core.AddEvent(newTakeover);
                    }
                    else
                    {
                        //ked pridavam do frontu, meni sa mi - robim statistiku
                        var _time = core.currentTime - ((STK)core).localAverageCustomerCountInLineToTakeOver.timeOfLastChange;
                        ((STK)core).localAverageCustomerCountInLineToTakeOver.addValues(((STK)core).getCustomersCountInLine(), _time);
                        ((STK)core).localAverageCustomerCountInLineToTakeOver.setFinalTimeOfLastChange(core.currentTime);

                        ((STK)core).addCustomerToLine(customer);
                    }
                    
                }
            }
            else
            {
                var _time = core.currentTime - ((STK)core).localAverageCustomerCountInLineToTakeOver.timeOfLastChange;
                ((STK)core).localAverageCustomerCountInLineToTakeOver.addValues(((STK)core).getCustomersCountInLine(), _time);
                ((STK)core).localAverageCustomerCountInLineToTakeOver.setFinalTimeOfLastChange(core.currentTime);

                ((STK)core).addCustomerToLine(customer);
            }

        }
    }
}
