using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace voteBatch
{
    public partial class Form1 : Form
    {
        modbus mb = new modbus();
        string port = Properties.Settings.Default["Port"].ToString();
        string baudrate = Properties.Settings.Default["Baudrate"].ToString();
        string DB  = Properties.Settings.Default["DB"].ToString();
        string slaveId = Properties.Settings.Default["SlaveId"].ToString();
        string[] myParameters;

        public Form1(string[] args)
        {
            InitializeComponent();
            myParameters = args;
            if (myParameters.Length <= 0) {
                this.Close();
                Environment.Exit(Environment.ExitCode);
            }
            string gender = args[0]; // 0:boy , 1:girl
            string signal = args[1]; // 1~5;

            if (gender != "0" && gender != "1") {
                gender = "0";
            }
            if (signal != "1" && signal != "2" && signal != "3" && signal != "4" && signal != "5") {
                signal = "1";
            }
            string g = (gender == "0") ? "男生" : "女生";
            string f = "";
            switch (signal) {
                case "1":
                    f = "很興奮";
                    break;
                case "2":
                    f = "好開心";
                    break;
                case "3":
                    f = "很難過";
                    break;
                case "4":
                    f = "好生氣";
                    break;
                case "5":
                    f = "Normal";
                    break;
            }
            MessageBox.Show(g+" : "+ f);
            // connection rs485
            //Open COM port using provided settings:
            if (mb.Open(port, Convert.ToInt32(baudrate), 8, Parity.None, StopBits.One))
            {
                // start send the signal to rs485
                // send boy 
                byte address = Convert.ToByte(slaveId);
                ushort start = Convert.ToUInt16(gender);
                short[] value = new short[1];
                value[0] = Convert.ToInt16(signal);

                try
                {
                    while (!mb.SendFc16(address, start, (ushort)1, value)) ;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    this.Close();
                    Environment.Exit(Environment.ExitCode);
                }
            }

            // close
            mb.Close();

            this.Close();
            Environment.Exit(Environment.ExitCode);
        }

    }
}
