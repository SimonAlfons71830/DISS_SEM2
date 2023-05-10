using DISS_SEM2.Events;
using DISS_SEM2.Objects.Cars;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DISS_SEM2.Core
{
    public class EventCore : MonteCarlo
    {
        //casova os 
        public SimplePriorityQueue<Event, double> timeline;
        //aktualny cas simulacie
        public double currentTime;
        private double maxTime;

        public override void Before()
        {
            //PPRED SIMULACIOU VYNULUJEM 
            if (((STK)this).getMode() == 2)
            {
                //vynulovat globalne statistiky
                ((STK)this).globalAverageCustomerTimeInSTK.resetStatistic();//1
                ((STK)this).globalAverageCustomerCountEndOfDay.resetStatistic();
                ((STK)this).globalAverageTimeToTakeOverCar.resetStatistic();
                ((STK)this).globalAverageCustomerCountInLineToTakeOver.resetStatistic();
                ((STK)this).globalAverageFreeTechnicianCount.resetStatistic();
                ((STK)this).globalAverageFreeAutomechanicCount.resetStatistic();
                ((STK)this).globalAverageCustomerCountEndOfDay.resetStatistic();
                ((STK)this).globalAverageCustomerCountEndOfDay.resetStatistic();

                ((STK)this).globalAverageCustomerCountInSTK.resetStatistic();
            }
            
        }

        public override void After()
        {

        }

        public override void BeforeReplication()
        {   //vynulovat lokalne statistiky
            ((STK)this).todaysCustomers = 0;
            if (((STK)this).getMode() == 2)
            {//fast
                ((STK)this).resetGarage();
                
                ((STK)this).resetTechnicians();
                ((STK)this).resetAutomechanics();
                ((STK)this).customerscount = 0;
                ((STK)this).currentTime = 0;

                ((STK)this)._ids = 0;
                ((STK)this).CarsCountGarage = 0;
                //((STK)this).resetQueues();

                ((STK)this).localAverageCustomerCountInLineToTakeOver.resetStatistic();
                ((STK)this).localAverageTimeToTakeOverCar.resetStatistic();
                ((STK)this).localAverageCustomerCountInLineToTakeOver.resetStatistic();
                ((STK)this).localAverageFreeTechnicianCount.resetStatistic();
                ((STK)this).localAverageFreeAutomechanicCount.resetStatistic();
                ((STK)this).localAverageCustomerCountInSTK.resetStatistic();
                ((STK)this).localAverageCustomerCountEndOfDay.resetStatistic();
            }
            ((STK)this).localAverageCustomerTimeInSTK.resetStatistic(); //1


            this.timeline = new SimplePriorityQueue<Event, double>();
            this.currentTime = 0;

            var newCustomer = new Customer(0, new Car(((STK)this).carTypeGenerator.Next()));
            ((STK)this).setId(newCustomer);
            this.AddEvent(new CustomerArrival(this, 0, newCustomer, null, null));

            if (((STK)this).getMode() == 1)
            {//slow mode

                //pridanie na 9:00
                //this.AddEvent(new ControlEvent(this, 0, null, null, null));
                this.AddEvent(new SystemEvent(this, 0, null, null, null));
            }
        }

        public override void AfterReplication()
        {
            this.replications++;

            if (((STK)this).getMode() == 2)
            {//fast
                this.currentTime = maxTime;
                ((STK)this).endCustomersWaiting();
                var pomcustom = ((STK)this).todaysCustomers;
                
                var meanofstatI = ((STK)this).localAverageCustomerTimeInSTK.getMean(); //1
                ((STK)this).globalAverageCustomerTimeInSTK.addValues(meanofstatI);//1

                ((STK)this).localAverageCustomerCountInLineToTakeOver.setFinalTimeOfLastChange(this.maxTime);
                ((STK)this).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(this.maxTime);
                ((STK)this).localAverageFreeAutomechanicCount.setFinalTimeOfLastChange(this.maxTime);
                ((STK)this).localAverageCustomerCountInSTK.setFinalTimeOfLastChange(this.maxTime);

                var pomC = ((STK)this).customerscount;
                ((STK)this).localAverageCustomerCountEndOfDay.addValues(((STK)this).customerscount);

                //((STK)this).endCustomersWaiting();

                ((STK)this).globalAverageTimeToTakeOverCar.addValues(((STK)this).localAverageTimeToTakeOverCar.getMean());
                var pomtime = ((STK)this).localAverageCustomerCountInLineToTakeOver.getMean();
                ((STK)this).globalAverageCustomerCountInLineToTakeOver.addValues(pomtime);
                ((STK)this).globalAverageFreeTechnicianCount.addValues(((STK)this).localAverageFreeTechnicianCount.getMean());
                ((STK)this).globalAverageFreeAutomechanicCount.addValues(((STK)this).localAverageFreeAutomechanicCount.getMean());
                ((STK)this).globalAverageCustomerCountEndOfDay.addValues(((STK)this).localAverageCustomerCountEndOfDay.getMean());
                ((STK)this).globalAverageCustomerCountInSTK.addValues(((STK)this).localAverageCustomerCountInSTK.getMean());
                ((STK)this).globalAverageCustomerCountEndOfDay.addValues(((STK)this).localAverageCustomerCountEndOfDay.getMean());

                ((STK)this).customersLineQ.Clear();

                ((STK)this).Notify();
            }
        }

        public override void Replication()
        {
            //dat rovno ako parameter replication poslem do konstruktora
            this.Simulate(((STK)this).getSimulationTime());
        }



        //kontrolujeme kazdych 5 min kolko je zakaznikov v rade ak tam niekto caka viac ako 3 min tak ho odtial vyhodime


        public void Simulate(double _maxTime)
        {
            this.maxTime = _maxTime;
            Event _event;
            while (this.timeline.Count != 0)
            {
                //vyberie si najmensi prvok z queue
                _event = this.timeline.Dequeue();
                //aktualny cas sa nastavi na cas eventu
                this.currentTime = _event.time; //getter
                if (((STK)this).getMode() == 1)
                { //slow mode
                    while (this.stop)
                    {
                        Thread.Sleep(1000);
                    }
                    ((STK)this).Notify();
                }
                //osetrenie casu -> uz sa dalsi event nevykona
                if (this.currentTime > this.maxTime)
                { 
                    break; 
                }
                _event.execute();
            }
            var pomCust = ((STK)this).customerscount;

        }

        public void AddEvent(Event _event)
        {
            this.timeline.Enqueue(_event, _event.time);
        }

    }
}
