using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class Exponential : Distribution<double>
    {
        private SeedGenerator seedGenerator;
        Random random;
        private double lambda;

        public Exponential(SeedGenerator _seedGenerator, double mi)
        {
            this.seedGenerator = _seedGenerator;
            this.lambda = 1 / mi; //1.0
            this.random = new Random(this.seedGenerator.generate_seed());

        }

        //https://www.eg.bucknell.edu/~xmeng/Course/CS6337/Note/master/node50.html
        // -LOG(1-Ri)/lambda -> v zatvorke generovane cislo od (0-1> , moze byt prepisane iba ako -LOG(Ri)/lambda <0,1) (INPUT ANALYZER OVERENE)
        public override double Next()
        {
            double x = -Math.Log(this.random.NextDouble()) / this.lambda;
            return x;
        }
    }
}
