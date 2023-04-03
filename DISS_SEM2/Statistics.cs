using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Statistics
    {
        private double sum;
        private double count;
        public Statistics()
        {
            this.sum = 0;
            this.count = 0;
        }

        public void addValues(double _sum)
        {
            this.sum += _sum;
            this.count++;
        }

        public double getMean()
        {
            return this.sum / this.count;
        }
        public double getSum() 
        { 
            return this.sum; 
        }
    }
}
