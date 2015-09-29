using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.SqlClient;
using System.IO;

namespace voteBatch
{
    public partial class Form1 : Form
    {
        modbus mb = new modbus();
        string port = Properties.Settings.Default["Port"].ToString();
        string baudrate = Properties.Settings.Default["Baudrate"].ToString();
        string db  = Properties.Settings.Default["connectionString"].ToString();
        string dbLocation = Properties.Settings.Default["dbLocation"].ToString();
        string slaveId = Properties.Settings.Default["SlaveId"].ToString();
        string boyRegister = "0";  //400001
        string girlRegister = "1"; //400002
        int retry = 1; //handle send error 

        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", dbLocation);

            preOpen();
            sendSignal();

            // close
            mb.Close();
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }

        private void sendSignal()
        {
            // signal have 5 type (0, 1, 2, 3, 4)
            string boySignal = "0";
            string girlSignal = "0";

            // get vote result from db
            using (SqlConnection con = new SqlConnection(db))
            {
                con.Open();

                string sql = @"SELECT TOP 5 [ID] ,[Gender], [Emotion] ,[Exciting] ,[Happy] ,[Sad] ,[Upset] ,[VotingDate] 
                               FROM [dbo].[VotingModels]
                               WHERE CONVERT(date, VotingDate) <= CONVERT(date, getdate()) AND Gender = 0
                               ORDER BY VotingDate DESC";
                using (SqlCommand command = new SqlCommand(sql, con))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(3) > 0)
                        {
                            boySignal = "0";
                            break;
                        }
                        else if (reader.GetInt32(4) > 0)
                        {
                            boySignal = "1";
                            break;
                        }
                        else if (reader.GetInt32(5) > 0)
                        {
                            boySignal = "2";
                            break;
                        }
                        else if (reader.GetInt32(6) > 0)
                        {
                            boySignal = "3";
                            break;
                        }
                    }
                }

                //Girl
                sql = @"SELECT TOP 5 [ID] ,[Gender], [Emotion] ,[Exciting] ,[Happy] ,[Sad] ,[Upset] ,[VotingDate] 
                        FROM [dbo].[VotingModels]
                        WHERE CONVERT(date, VotingDate) <= CONVERT(date, getdate()) AND Gender = 1
                        ORDER BY VotingDate DESC";
                using (SqlCommand command = new SqlCommand(sql, con))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(3) > 0)
                        {
                            girlSignal = "0";
                            break;
                        }
                        else if (reader.GetInt32(4) > 0)
                        {
                            girlSignal = "1";
                            break;
                        }
                        else if (reader.GetInt32(5) > 0)
                        {
                            girlSignal = "2";
                            break;
                        }
                        else if (reader.GetInt32(6) > 0)
                        {
                            girlSignal = "3";
                            break;
                        }
                     }
                }

            }
            // connection rs485 , open COM port using provided settings:
            if (mb.Open(port, Convert.ToInt32(baudrate), 8, Parity.None, StopBits.One))
            {
                // start send the signal to rs485
                // send boy 
                byte address = Convert.ToByte(slaveId);
                ushort start = Convert.ToUInt16(boyRegister);
                short[] value = new short[1];
                value[0] = Convert.ToInt16(boySignal);

                try
                {
                    while (!mb.SendFc16(address, start, (ushort)1, value)) ;
                }
                catch (Exception err)
                {
                    mb.Close();
                    // write error log
                    WriteToErrorLog("retry time : " + retry.ToString(), err.Message, "Send Signal Error");
                    // retry 3 times
                    retry++;
                    if (retry <= 3)
                    {
                        sendSignal();
                    }
                }

                // send girl 
                address = Convert.ToByte(slaveId);
                start = Convert.ToUInt16(girlRegister);
                value = new short[1];
                value[0] = Convert.ToInt16(girlSignal);

                try
                {
                    while (!mb.SendFc16(address, start, (ushort)1, value)) ;
                }
                catch (Exception err)
                {
                    mb.Close();
                    // write error log
                    WriteToErrorLog("retry time : " + retry.ToString(), err.StackTrace , "Send Signal Error");
                    // retry 3 times
                    retry++;
                    if (retry <= 3)
                    {
                        sendSignal();
                    }
                }
            }
            else
            { 
                // write error log
                WriteToErrorLog("retry time : " + retry.ToString(), "Connection RS484 Error", "Send Signal Error");
                retry++;
                if (retry <= 3)
                {
                    sendSignal();
                }
            }

        }

        private void preOpen()
        {
            mb.Open(port, Convert.ToInt32(baudrate), 8, Parity.None, StopBits.One);
        }

        private void WriteToErrorLog(string msg, string stkTrace, string title)
        {
            if (!(System.IO.Directory.Exists(Application.StartupPath + "\\Errors\\")))
            {
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\Errors\\");
            }

            FileStream fs = new FileStream(Application.StartupPath + "\\Errors\\errlog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter s = new StreamWriter(fs);
            s.Close();
            fs.Close();
            FileStream fs1 = new FileStream(Application.StartupPath + "\\Errors\\errlog.txt", FileMode.Append, FileAccess.Write);
            StreamWriter s1 = new StreamWriter(fs1);
            s1.Write("Title: " + title + "\r\n");
            s1.Write("Message: " + msg + "\r\n");
            s1.Write("StackTrace: " + stkTrace + "\r\n");
            s1.Write("Date/Time: " + DateTime.Now.ToString() + "\r\n");
            s1.Write("============================================" + "\r\n");
            s1.Close();
            fs1.Close();
        }

   }
}
