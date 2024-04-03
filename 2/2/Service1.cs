using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace _2
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)
        private string _processName = "Notepad";
        private string _processPath = "D:\\Notepad.exe";
        private DateTime[] _schedule = new[] { DateTime.Today.AddHours(2), DateTime.Today.AddHours(3) };

        public Service1()
        {
            InitializeComponent();
        }

        private void StartProcess(string processPath)
        {
            Process.Start(processPath);
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" +
           DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now + "\n");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);
            if (_schedule[0] < DateTime.Now && _schedule[1] > DateTime.Now)
            {
                Process[] _process = Process.GetProcessesByName("Notepad");
                if (_process.Length > 0)
                {
                    WriteToFile("Process is running, stop it!");
                    foreach (var process in Process.GetProcessesByName(_processName))
                    {
                        process.Kill();
                    }
                    WriteToFile("Process stopped at " + DateTime.Now);
                }

                else
                {
                    Console.WriteLine("ok");
                    WriteToFile("Process is not running, start it!");
                    StartProcess(_processPath);
                    WriteToFile("Process started at " + DateTime.Now);
                }
            }
            else
            {
                WriteToFile("Not schedule time!");
            }
        }
    }
}