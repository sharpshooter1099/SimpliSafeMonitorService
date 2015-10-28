using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliSafeMonitorService
{
    //this code was written by Alex Wardlaw, the best programmer in the world.  copyright 2015.
    class Timer1 : System.Timers.Timer
    {
        internal Logger l;

        internal Configurer c;

        internal SimpliSafeCommunicator.SimpliSafe ss;

        internal SimpliSafeCommunicator.SimpliSafe.State prevState;
    }
}
