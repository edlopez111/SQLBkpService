﻿
using System.ServiceProcess;

namespace SQLBkpService
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                
                new SQLBkpService()
             
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
