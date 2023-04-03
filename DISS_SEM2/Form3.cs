using DISS_SEM2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DISS_SEM2
{
    public partial class Form3 : Form
    {
        private STK _sim;
        public Form3(STK _simulationCore)
        {
            InitializeComponent();
            this._sim = _simulationCore;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("CHOOSE A MODE!");
            }
            else if (radioButton1.Checked)
            {
                var formSlow = new Form1(this._sim);
                this._sim.setMode(1);
                formSlow.ShowDialog();
            }
            else
            {
                var formFast = new Form2(this._sim);
                this._sim.setMode(2);
                formFast.ShowDialog();
            }
            
        }
    }
}
