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
            /*
            string fileName = "exp.txt";
            SeedGenerator seedgen = new SeedGenerator();
            Exponential exp = new Exponential(seedgen,12);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < 100000; i++)
                {
                    writer.WriteLine(exp.Next());
                }
            }
            */

            var stk = new STK();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(stk));
        }
    }
}
