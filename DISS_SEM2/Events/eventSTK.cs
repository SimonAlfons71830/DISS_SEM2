using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class eventSTK : Event
    {
        public Customer customer;
        public Technician technician;
        public Automechanic automechanic;

        public eventSTK(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) : base(_core, _time)
        {
            this.customer = _customer;
            this.technician = _technician;
            this.automechanic = _automechanic;
        }
    }
}
