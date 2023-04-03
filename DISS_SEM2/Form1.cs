using System;
using System.Data;
using System.Threading;
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
        DataTable dataTechnicians = new DataTable();
        DataTable dataAutomechanics = new DataTable();
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
            /*while (true)
            {
                if (!paused)
                {
                    break;
                }
                this._simCore.sleepSim();

            }*/
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
                label13.Text = _simulationCore.getReserverParkingPlaces().ToString() + "/" + "5";
                label16.Text = _simulationCore.getCarsCountInGarage().ToString() + "/" + "5";

                /*//datagrid technicians
                dataTechnicians.Clear();
                foreach (Technician technician in _simulationCore.getTechnicianList())
                {
                    DataRow row = dataTechnicians.NewRow();
                    row["Technician ID"] = technician._id;


                    Customer customer = technician.customer_car; // assuming this method returns the customer the technician is working on
                    if (customer != null)
                    {
                        row["Customer ID"] = customer._id;
                        row["Status"] = "Busy";
                    }
                    else
                    {
                        row["Status"] = "Free";
                    }

                    dataTechnicians.Rows.Add(row);
                }


                // Bind the DataTable to the DataGridView
                dataGridView1.DataSource = dataTechnicians;

                // Format the DataGridView
                dataGridView1.RowHeadersVisible = true;
                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // Format the "Status" column
                DataGridViewCellStyle busyStyle = new DataGridViewCellStyle();
                busyStyle.BackColor = Color.Red;
                busyStyle.ForeColor = Color.White;

                DataGridViewCellStyle freeStyle = new DataGridViewCellStyle();
                freeStyle.BackColor = Color.Green;
                freeStyle.ForeColor = Color.White;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Status"].Value != null && row.Cells["Status"].Value.ToString() == "Busy")
                    {
                        row.Cells["Status"].Style = busyStyle;
                    }
                    else
                    {
                        row.Cells["Status"].Style = freeStyle;
                    }
                }


                //datagrid automechanics
                dataAutomechanics.Clear();
                foreach (Automechanic automechanic in _simulationCore.getAutomechanicsList())
                {
                    DataRow row = dataAutomechanics.NewRow();
                    row["Automechanic ID"] = automechanic._id;


                    Customer customer = automechanic.customer_car; // assuming this method returns the customer the technician is working on
                    if (customer != null)
                    {
                        row["Customer ID"] = customer._id;
                        row["Status"] = "Busy";
                    }
                    else
                    {
                        row["Status"] = "Free";
                    }

                    dataAutomechanics.Rows.Add(row);
                }


                // Bind the DataTable to the DataGridView
                dataGridView2.DataSource = dataAutomechanics;

                // Format the DataGridView
                dataGridView2.RowHeadersVisible = true;
                dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // Format the "Status" column

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["Status"].Value != null && row.Cells["Status"].Value.ToString() == "Busy")
                    {
                        row.Cells["Status"].Style = busyStyle;
                    }
                    else
                    {
                        row.Cells["Status"].Style = freeStyle;
                    }
                }*/

            });
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this._simCore.addObserver(this);
            paused = true;
            _simTime = DateTime.Parse("2/16/2008 09:00:00 AM");
            oldtime = 0;

            DoubleBuffered = true;



            dataTechnicians.Columns.Add("Technician ID", typeof(int));
            dataTechnicians.Columns.Add("Customer ID", typeof(int));
            dataTechnicians.Columns.Add("Status", typeof(string));

            dataAutomechanics.Columns.Add("Automechanic ID", typeof(int));
            dataAutomechanics.Columns.Add("Customer ID", typeof(int));
            dataAutomechanics.Columns.Add("Status", typeof(string));


        }


        private void button1_Click(object sender, EventArgs e)
        {
            _simCore.SetSpeed((int)numericUpDown1.Value);

            _simCore.createAutomechanics((int)numericUpDown3.Value);
            _simCore.createTechnicians((int)numericUpDown2.Value);

            var time = (int)numericUpDown4.Value * 3600 - 1;
            _simCore.setSimulationTime(time);

            paused = false;
            if (numericUpDown2.Value == 0 || numericUpDown3.Value == 0 || numericUpDown4.Value == 0) 
            {
                MessageBox.Show("SET REQUIRED PARAMETERS.");
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
            this._simCore.Simulation(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //paused = true;
            _simCore.stop = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _simCore.stop = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
