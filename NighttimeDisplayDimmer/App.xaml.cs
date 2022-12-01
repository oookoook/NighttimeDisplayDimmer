using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using NighttimeDisplayDimmer.Detectors;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
            
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                p.ProcessName == proc.ProcessName).Count();

            if (count > 1)
            {
                //MessageBox.Show("Already an instance is running...");
                App.Current.Shutdown();
            }

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            tb = (TaskbarIcon)FindResource("taskbarIcon");
            
            NighttimeDetector.GetInstance().Initialize(Util.Config.GetInstance().RefreshInterval);
            DisplayChangeDetector.GetInstance().Initialize(Util.Config.GetInstance().RefreshInterval);
            

            // https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/send-local-toast?tabs=desktop-msix#step-3-handling-activation
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                Dispatcher.BeginInvoke(() =>
                {
                    NighttimeDisplayDimmer.MainWindow.HandleOpen();
                });

            };

            BrightnessChanger = new BrightnessChanger();

            // open the settings window if no displays are configured
            if (Util.Config.GetInstance().SavedDisplays.Count == 0)
            {
                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NighttimeDetector.GetInstance().Stop();
            DisplayChangeDetector.GetInstance().Stop();
            tb?.Dispose(); //the icon would clean up automatically, but this is cleaner
            Util.Config.GetInstance().Dispose();
            base.OnExit(e);
        }

    }
}
