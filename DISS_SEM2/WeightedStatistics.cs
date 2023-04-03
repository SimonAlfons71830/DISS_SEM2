using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class WeightedStatistics
    {
        private double timeOfLastChange;
        private double value;
        public WeightedStatistics()
        {
            this.value = 0;
            this.timeOfLastChange = 0;
        }

        public void addValues(double _count, double _timeOfLastChange)
        {
            this.value = _count * _timeOfLastChange;
        }

        public double getMean()
        {
            return this.value / this.timeOfLastChange;
        }

        public void setFinalTimeOfLastChange(double _time)
        { 
            this.timeOfLastChange = _time;
        }

        /*var cakanie = core.currentTime - (STK).timeOfLastChange;
        STK.statisticsChange.addValues(cakanie * STK.waitingLine());
        STK.timeOfLastChange = core.currentTime;*/
    }
}
