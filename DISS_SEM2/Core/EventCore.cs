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

        public override void BeforeReplication()
        {
            this.timeline = new SimplePriorityQueue<Event, double>();
            this.currentTime = 0;
            this.AddEvent(new CustomerArrival(this, 0, new Customer(0, new Car(((STK)this).carTypeGenerator.Next())), null,null));

        }
        public override void AfterReplication()
        {

        }

        public override void Replication()
        {
            this.Simulate(28800);
            //8 hodin
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
