using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Globalization;

namespace _4_Darbas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //getting path to desktop

        private async void GetProcessList()
        {
            progressBar1.Value = 0; //resetting bar value if used again
            Process[] processes = Process.GetProcesses();   //getting all processes
            var total = Math.Round(processes.Count() / (decimal)100.0); //counting how much to increment the loading bar
            var loop = 1;   //counter for loading bar

            textBox1.Text = "Writing to CurrentProcessList.txt...";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "CurrentProcessList.txt"))) //writing to a file
            {
                foreach (Process p in processes)
                {
                    outputFile.WriteLine("{0},{1}", p.Id, p.ProcessName);   //getting process id and name

                    if (loop % total == 0)
                    {
                        if (progressBar1.Value != 100)  //checking if the bar isn't full
                            progressBar1.Value++;
                    }
                    loop++;
                }
            }

            await Task.Delay(1000); //waiting for 1s
            textBox1.Text = "DONE!";

            if (progressBar1.Value != 100)
                progressBar1.Value = 100;         
        }

        private async void GetTop5Processes()
        {
            progressBar1.Value = 0;
            Process[] processes = Process.GetProcesses();

            textBox1.Text = "Getting process list...";

            var list = processes
                    .Where(process => process.ProcessName != "Idle")
                    .Select(process => process)
                    .OrderBy(process => process.TotalProcessorTime)
                    .Reverse()
                    .ToList().Take(5); //using LINQ to select top 5 processes

            progressBar1.Value = 50;
            DateTime localDate = DateTime.Now;  //getting current date
            var culture = new CultureInfo("lt-LT"); //using lithuanian format
            string date = "";

            await Task.Delay(1000);
            textBox1.Text = "Getting current date...";

            foreach (var s in localDate.ToString(culture))  //removing illegal characters for windows
            {
                if (s == ' ' || s == ':')
                {
                    date += '_';
                }
                else
                {
                    date += s;
                }
            }
            progressBar1.Value = 75;
            await Task.Delay(1000);
            textBox1.Text = "Writing to a file...";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, $"TopProcessList_{date}.csv"))) //writing to a file with current date and time
            {
                foreach (var v in list)
                {
                    outputFile.WriteLine($"{v.ProcessName}, {v.Id}, {v.TotalProcessorTime}");                
                }

            }
            progressBar1.Value = 100;
            await Task.Delay(1000);
            textBox1.Text = $"DONE! Check desktop for TopProcessList_{date}.csv";
        }

        public void GetNotepadProcess()
        {
            progressBar1.Value = 0;
            label1.MaximumSize = new Size(300, 300);    //setting new size for label for word wrapping
            label1.AutoSize = true;
            Process[] processes = Process.GetProcessesByName("Notepad");    //looking for notepad process
            if (!processes.Any())   //if the list is empty
            {
                label1.Text = "Process doesn't work.";
            }
            else
            {
                foreach (Process n in processes)
                {
                    label1.Text += $"Name: {n.ProcessName} | ID: {n.Id} | Is is responding: {n.Responding} | Memory: {n.PrivateMemorySize64}\n";    //get information about process             
                    n.Kill();   //kills the process
                }                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = ""; //resetting text
            label1.Text = "";   //resetting text
            GetProcessList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            label1.Text = "";
            GetTop5Processes();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            label1.Text = "";
            GetNotepadProcess();
        }
    }
}
