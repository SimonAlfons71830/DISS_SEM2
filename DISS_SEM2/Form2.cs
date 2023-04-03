using DISS_SEM2.Core;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DISS_SEM2
{
    public partial class Form2 : Form
    {
        private STK _simCore;
        private Thread thread1;
        public Form2(STK _simulationCore)
        {
            InitializeComponent();
            this._simCore = _simulationCore;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            _simCore.setSimulationTime(8*3600);
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
            this._simCore.Simulation((int)numericUpDown1.Value);
            this.Invoke((MethodInvoker)delegate
            {
                
                label9.Text = this._simCore.getStatI().ToString("0.0000"); // formatovanie na 4 desatinne cisla
                label10.Text = this._simCore.getStatII().ToString("0.0000");
                
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this._simCore.stop = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this._simCore.stop = false;
        }

       
    }
}
