using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SimpliSafeMonitorService
{
    //this code was written by Alex Wardlaw, the best programmer in the world.  copyright 2015.
    public partial class Scheduler : ServiceBase
    {
        internal Timer1 t2;

        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                t2 = new Timer1();
                t2.Interval = 300000;  //every 5 mins.

                //note, must create inherited timer class with instances of necessary variables to avoid null reference exception in tick event.
                t2.l = new Logger("SimpliSafeMonitorService");
                t2.c = new Configurer("SimpliSafeMonitorService");
                t2.l.WriteToLog("SimpliSafeMonitorService initialized");

                t2.ss = new SimpliSafeCommunicator.SimpliSafe(t2.c.activeFields["username"][0], t2.c.activeFields["password"][0]);

                t2.l.WriteToLog("dll initialized.");

                if (!t2.ss.Login())
                {
                    t2.l.WriteToLog("unable to log in.  stopping service.");
                    Environment.Exit(1);
                }

                t2.l.WriteToLog("successfully logged in.");

                if (!t2.ss.SetLocationIDByAddress(t2.c.activeFields["address"][0]))
                {
                    t2.l.WriteToLog("unable to set location ID by address.  stopping service.");
                    Environment.Exit(1);
                }
                t2.l.WriteToLog("successfully set location ID by address.");

                t2.prevState = t2.ss.GetStatus();
                t2.l.WriteToLog("current status is: " + t2.prevState);

                if (!t2.ss.Logout())
                {
                    t2.l.WriteToLog("error while logging out.  stopping service.");
                    Environment.Exit(1);
                }
                t2.l.WriteToLog(string.Format("logged out.  current state is {0}", t2.prevState));

                this.t2.Elapsed += new System.Timers.ElapsedEventHandler(this.t1_Tick);
                t2.Enabled = true;
            }
            catch (Exception ex)
            {
                t2.l.WriteToLog("an error occurred: " + ex.Message);
                Environment.Exit(1);
            }
        }

        private void t1_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckStatus(sender as Timer1);
        }

        protected override void OnStop()
        {
            t2.l.WriteToLog("service stopped");

            if (t2.ss.LoggedIn)
                t2.ss.Logout();

            t2.Stop();
            t2.Close();
            t2.Dispose();
        }

        private static string ExecuteProcess(string filePath, string arguments)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            // Start a new instance of this program but specify the 'spawned' version.
            System.Diagnostics.ProcessStartInfo myProcessStartInfo = new System.Diagnostics.ProcessStartInfo(filePath, arguments);
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo = myProcessStartInfo;
            myProcess.Start();
            System.IO.StreamReader myStreamReader = myProcess.StandardOutput;
            // Read the standard output of the spawned process. 
            return myStreamReader.ReadLine();
        }

        private void CheckStatus(Timer1 t1)
        {
            try
            {
                string[] filePath = t1.c.activeFields["FilePath"];
                string[] homeArgs = t1.c.activeFields["homeArgs"];
                string[] awayArgs = t1.c.activeFields["awayArgs"];
                string[] offArgs = t1.c.activeFields["offArgs"];

                t1.ss.Login();
                SimpliSafeCommunicator.SimpliSafe.State currState = t1.ss.GetStatus();
                t1.ss.Logout();

                if (currState != t1.prevState)
                {
                    switch (currState)
                    {
                        case SimpliSafeCommunicator.SimpliSafe.State.off:
                            t1.l.WriteToLog("status changed from: " + t1.prevState + " to " + currState);
                            for (int i = 0; i < filePath.Length && i < offArgs.Length; i++)
                                t1.l.WriteToLog(ExecuteProcess(filePath[i], offArgs[i]));
                            t1.prevState = currState;
                            break;
                        case SimpliSafeCommunicator.SimpliSafe.State.home:
                            t1.l.WriteToLog("status changed from: " + t1.prevState + " to " + currState);
                            for (int i = 0; i < filePath.Length && i < homeArgs.Length; i++)
                                t1.l.WriteToLog(ExecuteProcess(filePath[i], homeArgs[i]));
                            t1.prevState = currState;
                            break;
                        case SimpliSafeCommunicator.SimpliSafe.State.away:
                            t1.l.WriteToLog("status changed from: " + t1.prevState + " to " + currState);
                            for (int i = 0; i < filePath.Length && i < awayArgs.Length; i++)
                                t1.l.WriteToLog(ExecuteProcess(filePath[i], awayArgs[i]));
                            t1.prevState = currState;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                t1.l.WriteToLog("an error occurred inside the timer: " + ex.Message);
            }
        }
    }
}
