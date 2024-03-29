﻿namespace AutoSymSwitchService
{
    #region Namespaces
    using System.ComponentModel;
    using System.ServiceProcess;
    #endregion

    [RunInstaller(true)]
    public partial class AutoSymSwitchServiceInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;
        public AutoSymSwitchServiceInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "VAI_AutoSymSwitch";
            service.Description = "VAI Symetrix switcher";
            service.DelayedAutoStart = true;
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}