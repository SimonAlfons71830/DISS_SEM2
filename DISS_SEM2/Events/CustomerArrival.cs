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
            //prichod zakaznika vyvola dalsi prichod zakaznika - po 15 45 uz neplanujem
            //zistim freee technika
            //ak je free tak -> najprv paymentline 
            //ak nieje nikto v payment line tak kontrolujem dostupne parkovacie miesto
            // ak je free customerovi naplanuje takeover ak nieje free park miesto davam ho do rady
            // ak NEMAM free technika -> zakaznik ide do rady (prio je arrival time)

            var nextTime = ((STK)core).customerArrivalTimeGenerator.Next() + time;
            var nextCar = new Car(((STK)core).carTypeGenerator.Next());
            var nextCustomer = new Customer(nextTime, nextCar);
            ((STK)core).setId(nextCustomer);
            var nextArrival = new CustomerArrival(core,nextTime,nextCustomer,null,null);
            core.AddEvent(nextArrival);

            var technic = ((STK)core).getAvailableTechnician();
            // bud je to technic ktory neobsluhuje alebo null

            if (technic != null)
            {
                if (((STK)core).getCustomersCountInPaymentLine() > 0)
                {
                    var paymentTime = ((STK)core).paymentTimeGenerator.Next() + time;
                    var paymentCustomer = ((STK)core).getCustomerInPaymentLine();
                    ((STK)core).removeCustomerFromPaymentLine();
                    technic.obsluhuje = true;
                    var nextPayment = new Payment(core, paymentTime, paymentCustomer, technic, null);
                    core.AddEvent(nextPayment);
                }
                else
                {

                    if (((STK)core).getCarsCountInGarage() < 5)
                    {
                        var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                        technic.obsluhuje = true;
                        technic.customer_car = customer;
                        var newTakeover = new StartTakeOver(core, takeoverTime, customer, technic, null);
                        core.AddEvent(newTakeover);
                    }
                    else
                    {
                        ((STK)core).addCustomerToLine(customer);
                    }
                    
                }
            }
            else
            {
                ((STK)core).addCustomerToLine(customer);
            }






            /*
            var nextArrivalTime = ((STK)core).customerArrivalTimeGenerator.Next() + time;
            if (nextArrivalTime > 24300)
            {
                return; //15:45
            }
            core.AddEvent(new CustomerArrival(core, nextArrivalTime, new Customer(nextArrivalTime, new Car(((STK)core).carTypeGenerator.Next())),null,null));


            if (((STK)core).getCustomersCountInPaymentLine() == 0 && ((STK)core).getTechniciansCount() != 0)
            {
                //prebratie auta trva dlhsie a ludia chodia hned, tym padom 3ja dostanu toho isteho technika naraz
                //rovno ked najdem available tak ho nastavim na obsluhuje ak sa vytvara event 
                var technic = ((STK)core).getAvailableTechnician();
                if (technic != null && ((STK)core).getParkedCarsCountInGarage() <= 5)
                {
                    //trojuholnikove rozdelenie na prijatie auta a preparkovanie do garaze
                    var takeoverTime = ((STK)core).takeOverTimeGenerator.Next() + time;
                    technic.obsluhuje = true;
                    var startTakeOver = new StartTakeOver(core, takeoverTime, customer, technic, null);
                    core.AddEvent(startTakeOver);
                }
                else
                {
                    ((STK)core).addCustomerToLine(customer);
                }
            }
            else
            {
                ((STK)core).addCustomerToLine(customer);
            }*/
            


        }
    }
}
