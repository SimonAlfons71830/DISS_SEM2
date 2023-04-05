using DISS_SEM2.Core;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DISS_SEM2
{
    public partial class Form2 : Form, ISTKObserver<STK>
    {
        private STK _simCore;
        private Thread thread1;
        public Form2(STK _simulationCore)
        {
            InitializeComponent();
            this._simCore = _simulationCore;
        }

        public void refresh(STK simulationCore)
        {
            if (this.IsHandleCreated)
            {


                this.Invoke((MethodInvoker)delegate
                {
                    label3.Text = this._simCore.getActualReplication().ToString();

                    var statI = this._simCore.localAverageCustomerTimeInSTK.getMean() / 60;
                    if (statI == 0)
                    {
                        return;
                    }
                    label9.Text = statI.ToString("0.0000");
                    label10.Text = (this._simCore.localAverageTimeToTakeOverCar.getMean() / 60).ToString("0.0000"); //time to takeover car
                    label12.Text = this._simCore.localAverageCustomerCountInLineToTakeOver.getMean().ToString("0.0000");
                    label20.Text = this._simCore.localAverageFreeTechnicianCount.getMean().ToString("0.0000");
                    label21.Text = this._simCore.localAverageFreeAutomechanicCount.getMean().ToString("0.0000");
                    label17.Text = this._simCore.localAverageCustomerCountInSTK.getMean().ToString("0.0000");
                    label23.Text = this._simCore.localAverageCustomerCountEndOfDay.getMean().ToString("0.0000");

                });
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.reset();

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
            

            this._simCore.addObserver(this);
            _simCore.setSimulationTime(8 * 3600);
            this._simCore.Simulation((int)numericUpDown1.Value);

            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {

                    label9.Text = this._simCore.getStatI().ToString("0.0000"); // formatovanie na 4 desatinne cisla
                    label10.Text = this._simCore.getStatII().ToString("0.0000");
                    label12.Text = this._simCore.getStatIII().ToString("0.0000");
                    label20.Text = this._simCore.getStatIV().ToString("0.0000");
                    label21.Text = this._simCore.getStatV().ToString("0.0000");

                    var pom = this._simCore.globalAverageCustomerTimeInSTK.ConfidenceInterval(0.9);
                    label25.Text = "< " + (pom[0] / 60).ToString("0.0000") + " - " + (pom[1] / 60).ToString("0.0000") + " >";
                    label17.Text = this._simCore.getStatVI().ToString("0.0000");
                    var pom2 = this._simCore.globalAverageCustomerCountInSTK.ConfidenceInterval(0.95);
                    label24.Text = "< " + pom2[0].ToString("0.0000") + " - " + pom2[1].ToString("0.0000") + " >";

                    label23.Text = this._simCore.getStatVII().ToString("0.0000");
                });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {

                label9.Text = this._simCore.getStatI().ToString("0.0000"); // formatovanie na 4 desatinne cisla
                label10.Text = this._simCore.getStatII().ToString("0.0000");
                label12.Text = this._simCore.getStatIII().ToString("0.0000");
                label20.Text = this._simCore.getStatIV().ToString("0.0000");
                label21.Text = this._simCore.getStatV().ToString("0.0000");
                //confidence intervals
                var pom = this._simCore.globalAverageCustomerTimeInSTK.ConfidenceInterval(0.9);
                label25.Text = "< " + (pom[0] / 60).ToString("0.0000") + " - " + (pom[1] / 60).ToString("0.0000") + " >";
                label17.Text = this._simCore.getStatVI().ToString("0.0000");
                var pom2 = this._simCore.globalAverageCustomerCountInSTK.ConfidenceInterval(0.95);
                label24.Text = "< " + pom2[0].ToString("0.0000") + " - " + pom2[1].ToString("0.0000") + " >";
                //stat of customer count at the end of the day
                label23.Text = this._simCore.getStatVII().ToString("0.0000");
            });

            this._simCore.stop = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this._simCore.stop = false;
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void reset()
        {
            this._simCore.replications = 0;
            this._simCore.resetGarage();
            this._simCore.resetAutomechanics();
            this._simCore.resetTechnicians();
            this._simCore.resetQueues();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Interrupt();
            e.Cancel = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (thread1 != null || thread1.IsAlive)
            {
                thread1.Interrupt();
            }

            base.OnFormClosing(e);
        }
    }
}
