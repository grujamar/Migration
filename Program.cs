using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Migration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if DEBUG
            MigrationService servis = new MigrationService(); //ime servisa
            servis.onDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            #else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MigrationService()
            };
            ServiceBase.Run(ServicesToRun);
            #endif
        }
    }
}
