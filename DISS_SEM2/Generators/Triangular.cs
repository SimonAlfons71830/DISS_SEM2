﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class Triangular : Distribution<double>
    {
        private SeedGenerator seedGenerator;
        Random random;
        private double a;
        private double b;
        private double min;
        private double max;
        private double modus;
        private double range;
        private double mean;

        public Triangular(SeedGenerator _seedGenerator, double _min, double _max, double _modus)
        {
            this.seedGenerator = _seedGenerator;
            this.random = new Random(this.seedGenerator.generate_seed());

            this.min = _min;
            this.max = _max;
            this.modus = _modus;

            this.mean = (_min + _max + _modus) / 3;
            this.range = _max - _min;


        }

        //https://www.had2know.org/academics/triangular-distribution-random-variable-generator.html
        public override double Next()
        {
            this.a = random.NextDouble();
            this.b = random.NextDouble();

            if (this.a < (this.modus - this.min) / this.range)
                return this.min + Math.Sqrt(this.a * this.range * (this.modus - this.min));

            return this.max - Math.Sqrt((1 - this.a) * this.range * (this.max - this.modus));
        }


    }
}
