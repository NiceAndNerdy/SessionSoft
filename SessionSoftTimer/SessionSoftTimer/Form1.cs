using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SessionSoftTimer
{
    public partial class Form1 : Form
    {
        private Session CurrentSession;
        private ClientSettings Settings;
        private List<string> Processes = ClientSettings.GetProcesses();
        private SelectedPrinter CurrentPrinter = SelectedPrinter.Envisionware;

        private static string GetMac()
        {
            string mac =
               (
                   from nic in NetworkInterface.GetAllNetworkInterfaces()
                   where nic.OperationalStatus == OperationalStatus.Up
                   select nic.GetPhysicalAddress().ToString()
               ).FirstOrDefault();
            return mac;
        }

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            MainTimer.Interval = 1000;
            string mac = GetMac();
            CurrentSession = new Session(mac);
            if (CurrentSession.ID == 0)
            {
                Process.Start("IExplore.exe", "-k http://SSServer/SessionSoftWebClient/default.aspx?mac=" + mac);
            }
            else if (CurrentSession.ID == -1)
            {
                if (Environment.Is64BitOperatingSystem) { Process.Start("IExplore.exe", @"C:\Program Files (x86)\SessionSoft\ServerNotFound.html"); }
                else { Process.Start("IExplore.exe", @"C:\Program Files\SessionSoft\ServerNotFound.html"); }
            }
            else
            {
                Processes = ClientSettings.GetProcesses();
                Settings = new ClientSettings();
                CurrentSession.Time = Settings.MainTimerCount;
                Settings.GetAutoRenew(CurrentSession.ID);
                Process.Start("IExplore.exe", "-k http://SSServer/SessionSoftWebClient/default.aspx?id=" + CurrentSession.ID);
                MainTimer.Start();
            }
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            Process[] sssProcesses = Process.GetProcessesByName("SSS");
            if (sssProcesses.Length < 1)
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    Process.Start(@"C:\Program Files (x86)\SessionSoft\SSS.exe");
                }
                else { Process.Start(@"C:\Program Files\SessionSoft\SSS.exe"); }
            }

            CurrentSession.GetSession();

            if (CurrentSession.SelectedPrinter != CurrentPrinter)
            {
                switch (CurrentSession.SelectedPrinter)
                {
                    case SelectedPrinter.Envisionware:
                        runScript(PrinterScripts.MakeEnvisionWareDefault);
                        break;
                    case SelectedPrinter.CirculationDesk:
                        Process process = new System.Diagnostics.Process();
                        ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        if (Environment.Is64BitOperatingSystem) { startInfo.Arguments = PrinterScripts.AddCirculationPrinter; }
                        else { startInfo.Arguments = PrinterScripts.AddCirculationPrinterx86; }
                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                        runScript(PrinterScripts.MakeCirculationDefault);
                        break;
                }
                CurrentPrinter = CurrentSession.SelectedPrinter;
            }

            if ((CurrentSession.Message != "") && (CurrentSession.Message != "none"))
            {
                CurrentSession.Time--;
                groupBox1.Show();
                this.TopMost = true;
                this.WindowState = FormWindowState.Normal;
                lblMessage.Text = CurrentSession.Message;
                UpdateTimer();
            }
            else if ((CurrentSession.Status == "unlocked") || (CurrentSession.Status == "renewed"))
            {
                CurrentSession.Time--;
                if (CurrentSession.Time % Settings.Mod == 0)
                {
                    switch (CurrentSession.Time)
                    {
                        case 300:
                            TryRenew("This session will end in five minutes.  Please see a librarian if you want to renew.");
                            break;
                        case 60:
                            TryRenew("This session will end in one minute.  All unsaved work will be lost!");
                            break;
                        case 0:
                            TryRenew("Ending session.");
                            break;
                        default: break;
                    }
                }
                CheckIdle(false);
                UpdateTimer();
            }
            else if (CurrentSession.Status == "idle")
            {
                CurrentSession.Time--;
                CheckIdle(true);
                UpdateTimer();
            }
            else if (CurrentSession.Status == "terminate")
            {
                CurrentSession.EndSession(Settings);
                Process.Start("shutdown.exe", "/r /f /t 0");
            }
            else if (CurrentSession.Status == "locked")
            {
                this.WindowState = FormWindowState.Minimized;
                CurrentSession.EndSession(Settings);
                Process[] processes = Process.GetProcessesByName("IEXPLORE");
                if (processes.Length < 1)
                {
                    Process.Start("IExplore.exe", "-k http://SSServer/SessionSoftWebClient/default.aspx?id=" + CurrentSession.ID);
                }
            }
            else if ((CurrentSession.Status == "startkoha") || (CurrentSession.Status == "startstaff") || (CurrentSession.Status == "startsip")) { CurrentSession.StartSession(Settings, false, CurrentSession.Status, Processes); this.WindowState = FormWindowState.Normal; }
            else if (CurrentSession.Status == "startadmin") { CurrentSession.StartSession(Settings, true, CurrentSession.Status, Processes); }
            else if (CurrentSession.Status == "restart")
            {
                CurrentSession.EndSession(Settings);
                Process.Start("shutdown.exe", "/r /f /t 0");
            }
            else if (CurrentSession.Status == "shutdown")
            {
                CurrentSession.EndSession(Settings);
                Process.Start("shutdown.exe", "/s /f /t 0");
            }
            else if (CurrentSession.Status == "resume") { CurrentSession.ResumeSession(); }
            else if (CurrentSession.Status == "disabled") {
                this.FormClosing -= this.Form1_FormClosing;
                this.Close();
            }

            CurrentSession.UpdateServer();
        }


        private void UpdateTimer()
        {
            if (!(CurrentSession.Status == "idle") && ((CurrentSession.Message == "none") || (CurrentSession.Message == "")))  { groupBox1.Hide(); }
            if (CurrentSession.Time <= 0)
            {
                labelTime.Text = "00:00";
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(CurrentSession.Time);
                labelTime.Text = time.ToString(@"mm\:ss");
            }
        }

        private void CheckIdle(bool isIdle)
        {
            long milliseconds = IdleTimeFinder.GetIdleTime();

            if (isIdle)
            {
                if (milliseconds <= Settings.MaxIdleSplash)
                {
                    this.WindowState = FormWindowState.Minimized;
                    groupBox1.Hide();
                    CurrentSession.ResumeSession();
                }
                else
                {
                    CurrentSession.UntilRestartTime++;
                    if (CurrentSession.UntilRestartTime >= (Settings.MaxIdleRestart / (1000 / Settings.Mod)))
                    {
                        groupBox1.Hide();
                        CurrentSession.EndSession(Settings);
                        Process.Start("shutdown.exe", "/r /f /t 0");
                    }
                }
            }

            if (milliseconds >= Settings.MaxIdleSplash)
            {
                if (!isIdle)
                {
                    groupBox1.Show();
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Normal;
                    lblMessage.Text = "Still there?";
                    CurrentSession.GoIdle();
                    CurrentSession.GetSession();

                }
            }
        }

        private void TryRenew(string message)
        {
            Settings.GetAutoRenew(CurrentSession.ID);
            if (Settings.AutoRenew)
            {
                CurrentSession.RenewSession(Settings);
            }
            else
            {
                groupBox1.Show();
                lblMessage.Text = message;
                this.WindowState = FormWindowState.Normal;
            }
            if (CurrentSession.Time <= 0)
            {
                CurrentSession.EndSession(Settings);
                Process.Start("shutdown.exe", "/r /f /t 0");
            }
        }

        private void btnLogOff_Click(object sender, EventArgs e)
        {
            DialogResult response = MessageBox.Show("Are you sure?  This will reset the PC.  All unsaved work will be lost!", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (response == DialogResult.Yes)
            {
                CurrentSession.EndSession(Settings);
                Process.Start("shutdown.exe", "/r /f /t 0");
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnAcknowledge_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            this.WindowState = FormWindowState.Minimized;
            CurrentSession.Message = String.Empty;
            CurrentSession.UpdateServer();
        }

        private void runScript(String arguments)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = arguments;
            process.StartInfo = startInfo;
            process.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
