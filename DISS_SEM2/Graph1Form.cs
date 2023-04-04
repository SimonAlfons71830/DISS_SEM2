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

namespace DISS_SEM2
{
    public partial class Graph1Form : Form
    {
        private Thread thread1;
        private STK _simulationCore;
        public Graph1Form(STK _simCore)
        {
            InitializeComponent();
            this._simulationCore = _simCore;
        }

        private void Graph1Form_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            thread1 = new Thread(new ThreadStart(this.startSimulation));
            thread1.IsBackground = true;
            thread1.Start();

        }


        private void startSimulation()
        {
            this._simulationCore.setSimulationTime(8 * 3600);
            for (int i = 1; i <= 15; i++)
            {
                this._simulationCore.createTechnicians(i);
                this._simulationCore.createAutomechanics((int)numericUpDown2.Value);
                this._simulationCore.Simulation((int)numericUpDown1.Value);
                this.updateChart(i,this._simulationCore.localAverageCustomerCountInLineToTakeOver.getMean());
            }
            
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        public void updateChart (int numberOfTechnicians, double averageCustomers)
        {
            this.Invoke((MethodInvoker)delegate
            {
                chart1.Series["Series1"].Points.AddXY(numberOfTechnicians, averageCustomers);
                //time_chart.Update();
                chart1.Update();
            });
        }
        
    }
}
