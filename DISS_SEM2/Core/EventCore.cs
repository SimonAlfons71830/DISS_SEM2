﻿using DISS_SEM2.Events;
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

        public override void Before()
        {
            base.Before();
        }

        public override void After() 
        {
            base.After(); 
        }

        public override void BeforeReplication()
        {
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
            ((STK)this).globalAverageCustomerTimeInSTK.addValues(((STK)this).localAverageCustomerTimeInSTK.getMean());
            ((STK)this).globalAverageTimeToTakeOverCar.addValues(((STK)this).localAverageTimeToTakeOverCar.getMean());
        }

        public override void Replication()
        {
            //dat rovno ako parameter replication poslem do konstruktora
            this.Simulate(((STK)this).getSimulationTime());
        }

        public void Simulate(double maxTime)
        {
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
                if (this.currentTime > maxTime) { break; }
                _event.execute();
            }

        }

        public void AddEvent(Event _event)
        {
            this.timeline.Enqueue(_event, _event.time);
        }

    }
}
