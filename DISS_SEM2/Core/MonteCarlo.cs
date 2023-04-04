using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Core
{

    public abstract class MonteCarlo
    {
        public int replications;
        public bool stop { get; set; }

        private Stopwatch stopwatch;
        //vynulovanie hodnot
        public virtual void BeforeReplication() {
            
        }
        //hodnoty do grafu
        public virtual void AfterReplication() 
        {
            //zapisovat si do globalnych statistik priemery z lokalnych statistik 
        }
        //nastavit casomieru
        public virtual void Before()
        {
            this.replications = 0;   
        }
        //zastavit casomieru
        public virtual void After()
        {

        }

        public abstract void Replication();

        public void Simulation(int _numberOfReplications)
        {
            this.stop = false;
            this.Before();
            
            for (int i = 0; i < _numberOfReplications; i++) //input z textbox
            {
                while (stop)
                {
                    ((STK)this).sleepSim();
                    break;
                }
                this.BeforeReplication();
                this.Replication();
                this.AfterReplication();
            }
            this.After();

        }
    }
}
