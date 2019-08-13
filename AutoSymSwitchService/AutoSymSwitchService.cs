using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;

namespace AutoSymSwitchService
{
    public partial class AutoSymSwitchService : ServiceBase
    {
        ServiceHost oServiceHost = null;
        Timer timer = new Timer();

        public AutoSymSwitchService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            oServiceHost = new ServiceHost(typeof(AutoSymSwitch.AutoSymSwitchRestWCF));
            oServiceHost.Open();
            // timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            // timer.Interval = 5000; //number in milisecinds
            // timer.Enabled = true;
            new AutoSymSwitch.Logger().WriteToFile("Service VAI_AutoSymSwitch started");
        }

        protected override void OnStop()
        {
            new AutoSymSwitch.Logger().WriteToFile("Service VAI_AutoSymSwitch stopped");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            new AutoSymSwitch.Logger().WriteToFile("Service VAI_AutoSymSwitch recall");
        }
    }
}
