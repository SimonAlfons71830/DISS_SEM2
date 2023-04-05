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
            if (!radioButton1.Checked && !radioButton2.Checked && !radioButton3.Checked && !radioButton4.Checked)
            {
                MessageBox.Show("CHOOSE A MODE!");
            }
            else if (radioButton1.Checked)
            {
                var formSlow = new Form1(this._sim);
                this._sim.setMode(1);
                formSlow.ShowDialog();
            }
            else if (radioButton2.Checked)
            {
                var formFast = new Form2(this._sim);
                this._sim.setMode(2);
                formFast.ShowDialog();

            }
            else if (radioButton3.Checked) 
            {
                var formGraph1 = new Graph1Form(this._sim);
                this._sim.setMode(2);
                formGraph1.ShowDialog();    
            }
            else //(radioButton4.Checked)
            { 
                var formGraph2 = new Graph2Form(this._sim);
                this._sim.setMode(2);
                formGraph2.ShowDialog();
            }


            
        }

        
    }
}
