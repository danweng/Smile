﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

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
			File.AppendAllText(@"c:\temp\kevintan.txt", "1\r\n");
            // connection rs485
            //Open COM port using provided settings:
            if (mb.Open(port, Convert.ToInt32(baudrate), 8, Parity.None, StopBits.One))
            {
				File.AppendAllText(@"c:\temp\kevintan.txt", "2\r\n");
                // start send the signal to rs485
                // send boy 
                byte address = Convert.ToByte(slaveId);
                ushort start = Convert.ToUInt16(gender);
                short[] value = new short[1];
                value[0] = Convert.ToInt16(signal);
				
                try
                {
					File.AppendAllText(@"c:\temp\kevintan.txt", "3\r\n");
                    while (!mb.SendFc16(address, start, (ushort)1, value)) ;
					File.AppendAllText(@"c:\temp\kevintan.txt", "4\r\n");
                }
                catch (Exception err)
                {
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
