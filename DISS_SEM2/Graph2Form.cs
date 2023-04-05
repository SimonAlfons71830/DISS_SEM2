﻿using DISS_SEM2.Core;
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
    public partial class Graph2Form : Form
    {
        private Thread thread1;
        private STK _simulationCore;
        public Graph2Form(STK _simCore)
        {
            InitializeComponent();
            this._simulationCore = _simCore;
        }

        private void Graph2Form_Load(object sender, EventArgs e)
        {
            time_chart.Series["Dependance"].BorderWidth = 3;
            time_chart.DataBind();
        }

        

        private void startSimulation()
        {
            this._simulationCore.setSimulationTime(8 * 3600);
            for (int i = 10; i <= 25; i++)
            {
                this._simulationCore.createAutomechanics(i);
                this._simulationCore.createTechnicians((int)numericUpDown2.Value);
                this._simulationCore.Simulation((int)numericUpDown1.Value);
                this.updateChart(i, this._simulationCore.localAverageCustomerTimeInSTK.getMean()/60);
            }

        }

        public void updateChart(int numberOfAutomechanics, double averageTime)
        {
            this.Invoke((MethodInvoker)delegate
            {
                time_chart.Series["Dependance"].Points.AddXY(numberOfAutomechanics, averageTime);
                //time_chart.Update();
                time_chart.Update();
            });
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            time_chart.Series["Dependance"].Points.Clear();
            this._simulationCore.resetSim();

            thread1 = new Thread(new ThreadStart(this.startSimulation));
            thread1.IsBackground = true;
            thread1.Start();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            /*if (thread1 != null && thread1.IsAlive)
            {
                thread1.Interrupt();
            }
*/
            base.OnFormClosing(e);
        }
    }
}
