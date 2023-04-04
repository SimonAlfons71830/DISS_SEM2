using DISS_SEM2.Events;
using DISS_SEM2.Objects.Cars;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //vynulovat globalne statistiky
            ((STK)this).globalAverageCustomerTimeInSTK.resetStatistic();
            ((STK)this).globalAverageCustomerCountEndOfDay.resetStatistic();
            ((STK)this).globalAverageTimeToTakeOverCar.resetStatistic();
            ((STK)this).globalAverageCustomerCountInLineToTakeOver.resetStatistic();
            ((STK)this).globalAverageFreeTechnicianCount.resetStatistic();
            ((STK)this).globalAverageFreeAutomechanicCount.resetStatistic();
            ((STK)this).globalAverageCustomerCountEndOfDay.resetStatistic();
        }

        public override void After() 
        {
            
        }

        public override void BeforeReplication()
        {   //vynulovat lokalne statistiky
            if (((STK)this).getMode() == 2)
            {//fast
                ((STK)this).localAverageCustomerTimeInSTK.resetStatistic();
                ((STK)this).localAverageCustomerCountEndOfDay.resetStatistic();
                ((STK)this).localAverageTimeToTakeOverCar.resetStatistic();
                ((STK)this).localAverageCustomerCountInLineToTakeOver.resetStatistic();
                ((STK)this).localAverageFreeTechnicianCount.resetStatistic();
                ((STK)this).localAverageFreeAutomechanicCount.resetStatistic();
                ((STK)this).localAverageCustomerCountEndOfDay.resetStatistic();
                ((STK)this).localAverageCustomerCountInSTK.resetStatistic();
            }



            this.timeline = new SimplePriorityQueue<Event, double>();
            this.currentTime = 0;

            var newCustomer = new Customer(0, new Car(((STK)this).carTypeGenerator.Next()));
            ((STK)this).setId(newCustomer);
            this.AddEvent(new CustomerArrival(this, 0, newCustomer, null, null));
            if (((STK)this).getMode() == 1)
            {//slow mode
                this.AddEvent(new SystemEvent(this, 0, null, null, null));
            }

        }
        public override void AfterReplication()
        {
            this.replications++;

            if (((STK)this).getMode() == 2)
            {//fast

               /* if (this.timeline.Count != 0)
                {
                    //dokonci 
                }*/


                ((STK)this).localAverageCustomerCountInLineToTakeOver.setFinalTimeOfLastChange(this.maxTime);
                ((STK)this).localAverageFreeTechnicianCount.setFinalTimeOfLastChange(this.maxTime);
                ((STK)this).localAverageFreeAutomechanicCount.setFinalTimeOfLastChange(this.maxTime);
                ((STK)this).localAverageCustomerCountInSTK.setFinalTimeOfLastChange(this.maxTime);


                var meanofstatI = ((STK)this).localAverageCustomerTimeInSTK.getMean();
                if (meanofstatI == 0)
                {
                    return; //?????
                }

                ((STK)this).globalAverageCustomerTimeInSTK.addValues(meanofstatI);
                ((STK)this).globalAverageTimeToTakeOverCar.addValues(((STK)this).localAverageTimeToTakeOverCar.getMean());
                ((STK)this).globalAverageCustomerCountInLineToTakeOver.addValues(((STK)this).localAverageCustomerCountInLineToTakeOver.getMean());
                ((STK)this).globalAverageFreeTechnicianCount.addValues(((STK)this).localAverageFreeTechnicianCount.getMean());
                ((STK)this).globalAverageFreeAutomechanicCount.addValues(((STK)this).localAverageFreeAutomechanicCount.getMean());
                ((STK)this).globalAverageCustomerCountEndOfDay.addValues(((STK)this).localAverageCustomerCountEndOfDay.getMean());
                ((STK)this).globalAverageCustomerCountInSTK.addValues(((STK)this).localAverageCustomerCountInSTK.getMean());

                ((STK)this).Notify();
            }
            


        }

        public override void Replication()
        {
            //dat rovno ako parameter replication poslem do konstruktora
            this.Simulate(((STK)this).getSimulationTime());
        }

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
                    ((STK)this).Notify();
                }
                //osetrenie casu -> uz sa dalsi event nevykona
                if (this.currentTime > this.maxTime) { break; }
                _event.execute();
            }

        }

        public void AddEvent(Event _event)
        {
            this.timeline.Enqueue(_event, _event.time);
        }

    }
}
