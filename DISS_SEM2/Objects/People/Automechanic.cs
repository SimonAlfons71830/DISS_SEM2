using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Automechanic
    {
        public Car car;
        public bool obsluhuje;

        public Automechanic()
        {
            this.obsluhuje = false;
        }

        public bool Obsluhuje()
        { return this.obsluhuje; }

    }
}
