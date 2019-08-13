using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymSwitchService
{
    static class Program
    {
        static void Main()
        {
            /* CMR TODO
             * Devolución de jsons con status + texto de error
             * Código 404 / 500 por defecto con json
             * Habilitar HTTPS
             * Migrar todo a POST en vez de GET
             */

#if DEBUG
            AutoSymSwitchService myservice = new AutoSymSwitchService();
            myservice.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AutoSymSwitchService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
