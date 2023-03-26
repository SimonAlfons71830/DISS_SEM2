using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Technician
    {
        public Car car;
        public bool obsluhuje;

        public Technician() 
        {
            this.obsluhuje = false;
        }

        public bool Obsluhuje()
        { return this.obsluhuje; } 
            
    }
}
