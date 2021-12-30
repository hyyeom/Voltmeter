using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Voltmeter
{
    public partial class Form1 : Form
    {
        Thread rV_Thread;
        delegate void SetTextCallback(string text);

        double voltage = 0.00;
        double v1;
        float fullscale = 5;
        int data0, data1;
        string voltString;
        Boolean runa = true;
        string path = @"D:\_Data\";
        int cnt = 0;
        string ss = "";
        FileStream fs;
        StreamWriter tw;
        private void SetText(string text)
        {
            if (this.tbVolt.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.tbVolt.Text = text;
            }
        }
        public Form1()
        {
            InitializeComponent();
            getAvailablePorts();
            
        }

        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames();
            cbPort.Items.AddRange(ports);
        }

        public void readVoltage()
        {
            while (runa)
            {
                try {

                    data0 = Convert.ToInt32(serialPort1.ReadLine());
                    data1 = Convert.ToInt32(serialPort1.ReadLine());
                    v1 = (data0 * 256.0 + data1) / 1024.0 * fullscale;
                    voltage = Math.Round(v1, 2);
                    voltString = String.Format("{0:0.00}", voltage);
                    //tbVolt.Text = voltage.ToString();

                    double d = cnt / 100.0;
                    tw.WriteLine(d + ", " + voltString);
                    SetText(voltString);
                    cnt++;
                    //Task.Delay(900);
                } catch
                {

                }
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            runa = true;
            string sn = path + "voltage.txt";
            fs = new FileStream(sn, FileMode.Append, FileAccess.Write);
            tw = new StreamWriter(fs);
            cnt = 0;
            try
            {
                if ((cbPort.Text == "" || cbBaud.Text == ""))
                {
                    tbMessage.Text = "Please select port settings";
                }
                else if (serialPort1.IsOpen)
                {
                    rV_Thread = new Thread(new ThreadStart(readVoltage));
                    //ThreadStart rV = new ThreadStart(readVoltage);
                    //rV_Thread = new Thread(rV);
                    rV_Thread.Start();
                    //readVoltage();
                }
                else
                {
                    serialPort1.PortName = cbPort.Text;
                    serialPort1.BaudRate = Convert.ToInt32(cbBaud.Text);
                    serialPort1.Open();
                    rV_Thread = new Thread(new ThreadStart(readVoltage));
                    rV_Thread.Start();
                }
            }
            catch (Exception ex)
            {
                tbMessage.Text = "Unauthorized Access    " + ex.Message;
            }
        }

        private void cbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbMessage.Text = "Port Selected";
        }

        private void cbBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbMessage.Text = "Baud Selected";
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            tw.Close();
            runa = false;
            //serialPort1.WriteLine("0");
            rV_Thread.Abort();
            serialPort1.Close();
            Application.Exit();
        }
    }
}
