using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DISS_SEM2.Events
{
    internal class ControlEvent: eventSTK
    {
        public ControlEvent(EventCore _core, double _time, Customer _customer, Technician _technician, Automechanic _automechanic) :
            base(_core, _time, _customer, _technician, _automechanic)
        {
        }
        public override void execute()
        {
            //naplanuje sa na kazdych 5 min
            var nextTime = 5 + time; // po sekunde
            //kontrolujem zakaznikov v rade na prevzatie auta
            var pocet = ((STK)core).getCustomersCountInLine();
            //ak je tam niekto viac ako 3 min tak ho vyhodim
            //var pomList = ((STK)core).getCustomersInLine();

            for (int i = 0; i < pocet; i++)
            {
                var itycustomer = ((STK)core).customersLineQ.ElementAt(0); 
                //ak sa nevymaze tak nemozem ist od 0 lebo mi bude stale toho isteho 
                //ak su zoradeni podla arrival time tak uz dalsieho nevymaze 

                if (core.currentTime - itycustomer.arrivalTime > 3*60)
                {
                    
                    ((STK)core).customersLineQ.Remove(itycustomer);
                    //removnem tak sa zmeni queue - musim si niekde poznacit lebo queue musi este pokracovat prehladavat
                    //pocet = ((STK)core).getCustomersCountInLine();
                }
            }
            core.AddEvent(new ControlEvent(core, nextTime, null, null, null));
        }
    }
}
