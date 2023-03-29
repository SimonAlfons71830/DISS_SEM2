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
using DISS_SEM2.Core;

namespace DISS_SEM2
{
    public partial class Form1 : Form, ISTKObserver<STK>
    {
        STK _simCore;
        DateTime _simTime;
        private Thread thread1;
        private double oldtime;
        private bool paused;
        public Form1(STK _simulationCore)
        {
            this._simCore = _simulationCore;
            InitializeComponent();
            //_simTime = DateTime.Today.AddHours(9);
            //label4.Text = _simTime.ToString("hh:mm:ss tt");
        }

        public void refresh(STK _simulationCore)
        {
            _simCore.SetSpeed((int)numericUpDown1.Value);
            while (true)
            {
                if (!paused)
                {
                    break;
                }
                this._simCore.sleepSim();

            }
            //currecnt time v sim sa stale updatuje
            //nemozem pripocitat zakazdym novsi cas
            //pripocitat rozdiel medzi _simTime a current time do _simtime

            var time = _simCore.currentTime - oldtime;
            this.oldtime = _simCore.currentTime;
            _simTime = _simTime.AddSeconds(time);
            var customersLine = _simulationCore.getCustomersCountInLine().ToString();
            var customerspaymantline = _simulationCore.getCustomersCountInPaymentLine().ToString();
            this.Invoke((MethodInvoker)delegate {
                label4.Text = _simTime.ToString("hh:mm:ss tt");

                label5.Text = customersLine;
                label7.Text = customerspaymantline;
                label9.Text = _simulationCore.getFreeTechnicianCount().ToString() + "/" + _simulationCore.getAllTechniciansCount().ToString();
                label11.Text = _simulationCore.getFreeAutomechanicCount().ToString() + "/" + _simulationCore.getAllAutomechanicCount().ToString();

                label13.Text = _simulationCore.getFreeSpacesInGarage().ToString() + "/" + "5";
            });
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this._simCore.addObserver(this);
            paused = true;
            _simTime = DateTime.Parse("2/16/2008 09:00:00 AM");
            oldtime = 0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _simCore.SetSpeed((int)numericUpDown1.Value);

            paused = false;
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Before start choose a way of showing simulation and speed.");
            }
            else if (radioButton1.Checked)
            {
                thread1 = new Thread(new ThreadStart(this.startSimulation));
                thread1.IsBackground = true;
                thread1.Start();
            }else
            {
                var fastMode = new Form2(this._simCore);
                fastMode.ShowDialog();
            }
        }

        private void startSimulation()
        {
            this._simCore.Simulation();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            paused = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            paused = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        
    }
}
