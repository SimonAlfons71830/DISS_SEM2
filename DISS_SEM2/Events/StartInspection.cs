using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class StartInspection : eventSTK
    {
        public StartInspection(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) : base(_core, _time, _customer, _technician, _automechanic)
        {
        }
        public override void execute()
        {
            automechanic.obsluhuje = true;

            double timeOfInspection; ;
            if (automechanic.customer_car.getCar().type == Objects.Cars.CarTypes.Personal)
            {
                timeOfInspection = ((STK)core).personalCarInspectionGenerator.Next();
            }
            else if (automechanic.customer_car.getCar().type == Objects.Cars.CarTypes.Van)
            {
                timeOfInspection = ((STK)core).vanCarInspectionGenerator.Next();
            }
            else
            {
                timeOfInspection = ((STK)core).cargoCarInspectionGenerator.Next();
            }

            var endInspection = new EndInspection(core, timeOfInspection, customer, null, automechanic);
            core.AddEvent(endInspection);
        }
    }
}
