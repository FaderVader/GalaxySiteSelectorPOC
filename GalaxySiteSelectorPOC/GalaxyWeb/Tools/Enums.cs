using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalaxyWeb.Tools
{
    public class Enums
    {
        public enum TargetServiceState
        {
            StopService = 0,
            StartService = 1,
            RestartService = 2,
            NotFound = 99
        }

        public enum TargetProcessState
        {
            Stop = 0,
            Run = 1,
            NotFound = 99
        }
    }
}
