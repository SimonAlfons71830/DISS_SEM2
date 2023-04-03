using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace DISS_SEM2
{
    public partial class Form2 : Form
    {
        private STK _simCore;
        private Thread thread1;
        private Statistics stat1;
        public Form2(STK _simulationCore)
        {
            InitializeComponent();
            this._simCore = _simulationCore;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            _simCore.setSimulationTime(8*3600);
            this.stat1 = new Statistics();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            _simCore.createAutomechanics((int)numericUpDown3.Value);
            _simCore.createTechnicians((int)numericUpDown2.Value);

            if (numericUpDown1.Value == 0 || numericUpDown2.Value == 0 || numericUpDown3.Value == 0) 
            {
                MessageBox.Show("SET REQUIRED PARAMETERS");
            }
            else
            {
                thread1 = new Thread(new ThreadStart(this.startSimulation));
                thread1.IsBackground = true;
                thread1.Start();
            }
        }

        private void startSimulation()
        {
            for (int i = 0; i < (int)numericUpDown1.Value; i++)
            {
                this._simCore.Simulation();
                var value = this._simCore.getStatI();
                this.stat1.addValues(value);
            }
            this.Invoke((MethodInvoker)delegate
            {
                var seconds = this.stat1.getMean();
                var minutes = seconds / 60;
                label9.Text = minutes.ToString("00.000");
            });
        }

    }
}
