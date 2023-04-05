using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Intervals
    {
        private double sumOfValues;
        private int counter;
        private double sumOfSquaredValues;

        public Intervals() 
        {
            this.sumOfValues = 0;
            this.counter = 0;
            this.sumOfSquaredValues = 0;
        }

        private double standardDeviation()
        {
            var s = Math.Sqrt((this.sumOfValues - (Math.Pow(this.sumOfValues, 2) / this.counter)) / this.counter - 1); 

            return s;

        }


        public double[] ConfidenceInterval(double probability)
        {
            var zScore = this.GetZScore(probability);
            var s  = this.standardDeviation();
            var average = this.sumOfValues / this.counter;
            var margin = (s * zScore) / Math.Sqrt(this.counter);

            
            var lowerLimit = average - margin;
            var upperLimit = average + margin;

            return new double[] { lowerLimit, upperLimit };
        }

        private double GetZScore(double probability)
        {
            double zScore = 0.0;

            if (probability == 0.9)
                zScore = 1.645;
            else if (probability == 0.95)
                zScore = 1.96;
            else if (probability == 0.99)
                zScore = 2.576;
            else
                throw new ArgumentException("Invalid probability value. Supported values are 0.9, 0.95 and 0.99.");

            return zScore;
        }

        public void resetValues()
        {
            this.sumOfValues = 0;
            this.sumOfSquaredValues = 0;
            this.counter = 0;
        }

    }
}
