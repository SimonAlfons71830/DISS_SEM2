using DISS_SEM2.Core;
using DISS_SEM2.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DISS_SEM2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void Main()
        {

            /*string fileName = "triangular2.txt";
            SeedGenerator seedgen = new SeedGenerator();
            Triangular triangular = new Triangular(seedgen, 180, 695, 431);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < 1000000; i++)
                {
                    writer.WriteLine(triangular.Next());
                }
            }*/


            var stk = new STK();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form3(stk));
        }
    }
}
