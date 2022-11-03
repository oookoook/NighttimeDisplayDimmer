using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NighttimeDisplayDimmer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? tb;
        private BrightnessChanger? BrightnessChanger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            tb = (TaskbarIcon)FindResource("taskbarIcon");

            // TODO load interval from settings
            NighttimeDetector.GetInstance().Initialize(Util.Config.RefreshInterval);
            BrightnessChanger = new BrightnessChanger();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NighttimeDetector.GetInstance().Stop();
            tb?.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

    }
}
