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
        public int pocetreplikacii;
        public bool stop { get; set; }

        private Stopwatch stopwatch;
        //vynulovanie hodnot
        public virtual void BeforeReplication() { }
        //hodnoty do grafu
        public virtual void AfterReplication() 
        {
            //zobrazit na gui vysledky 
        }
        //nastavit casomieru
        public virtual void Before()
        {
            //stopwatch = new Stopwatch();
            //stopwatch.Start();
        }
        //zastavit casomieru
        public virtual void After()
        {
            //stopwatch.Stop();
            //var time = stopwatch.ElapsedMilliseconds;
        }

        public abstract void Replication();

        public void Simulation()
        {
            this.Before();
            
            this.BeforeReplication();
            this.Replication();
            this.AfterReplication();
            this.After();
        }
    }
}
