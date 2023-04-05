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
    public partial class Graph1Form : Form
    {
        private Thread thread1;
        public ManualResetEvent pauseEvent1 = new ManualResetEvent(true);
        private STK _simulationCore;
        public Graph1Form(STK _simCore)
        {
            InitializeComponent();
            this._simulationCore = _simCore;
        }

        private void Graph1Form_Load(object sender, EventArgs e)
        {

            time_chart.Series["Dependance"].BorderWidth = 3;
            time_chart.DataBind();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            time_chart.Series["Dependance"].Points.Clear();

            this._simulationCore.resetSim();

            time_chart.Series["Dependance"].Points.Clear();
            this._simulationCore.resetSim();
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

        public void updateChart (int numberOfTechnicians, double averageCustomers)
        {
            this.Invoke((MethodInvoker)delegate
            {
                time_chart.Series["Dependance"].Points.AddXY(numberOfTechnicians, averageCustomers);
                //time_chart.Update();
                time_chart.Update();
            });
        }

        private void time_chart_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            pauseEvent1.Reset();
        }
    }
}
