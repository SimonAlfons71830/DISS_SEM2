using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DISS_SEM2.Core;

namespace DISS_SEM2.Core
{
    public abstract class Event
    {
        //referencia na jadro
        public EventCore core;
        //cas kedy ma udalost nastat
        public double time { get; set; }

        public Event(EventCore _core, double _time)
        {
            this.core = _core;
            this.time = _time;
        }


        public virtual void execute() { }
    }
}
