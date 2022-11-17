using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Monitorian.Core.Models.Monitor;
using NighttimeDisplayDimmer.Detectors;
using Windows.Storage.Search;

namespace NighttimeDisplayDimmer
{
    internal class OptionsViewModel : INotifyPropertyChanged
    {
        private bool? nightModeEnabled;
        public ObservableCollection<MonitorInfo> Displays { get; }
        public bool? NightModeEnabled { get => nightModeEnabled; set { nightModeEnabled = value; NotifyPropertyChanged(); NotifyPropertyChanged("NightModeDisabled"); } }

        public bool? NightModeDisabled { get => !nightModeEnabled; }

        private bool loading = false;
        public bool Loading { get => loading; set { loading = value; NotifyPropertyChanged(); } }

        public IEnumerable<MonitorInfo> ManagedDisplays { get => Displays.Where(d => d.Enabled && (d.Supported ?? false)); }

        public bool StartOnLogin { get => Util.Startup.Automatic; set { Util.Startup.Automatic = value; NotifyPropertyChanged(); } }

        public bool StartOnLoginAllowed { get => Util.Startup.Allowed; }
        public bool EnableNotifications { get => Util.Config.GetInstance().EnableNotifications; set { Util.Config.GetInstance().EnableNotifications = value; } }
           
        public string HelpUrl { get => Util.Links.Help; }
        public string LicenseUrl { get => Util.Links.License; }

        public bool IsDirty { get {
                List<MonitorInfo> saved = Util.Config.GetInstance().SavedDisplays;
                /*
                // this means nothing - there are saved monitors that are not connected right now
                if(saved.Count != ManagedDisplays.Count())
                {
                    return true;
                }
                
                for(int i = 0; i < saved.Count; i++)
                {
                    if(!saved[i].Equals(ManagedDisplays.ElementAt(i)))
                    {
                        return true;
                    }
                }
                */

                // find out if every managed is in saved
                foreach (MonitorInfo monitor in ManagedDisplays)
                {
                    MonitorInfo? sm = saved.Find(d => d.DeviceInstanceId == monitor.DeviceInstanceId);
                    if(sm == null)
                    {
                        // adding new managed monitor
                        return true;
                    } else
                    {
                        // is so, check for equality
                        if (!monitor.Equals(sm))
                        {
                            return true;
                        }
                    }
                }

                return false;
            } }

        public string Version {
            get {
                Windows.ApplicationModel.PackageVersion v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}";
            } 
        }

        public OptionsViewModel()
        {
            NighttimeDetector d = NighttimeDetector.GetInstance();
            d.NightModeChanged += NightModeChanged;
            NightModeEnabled = d.NightMode;
            Displays = new ObservableCollection<MonitorInfo>();
        }

        private void NightModeChanged(object sender, NighttimeDetector.NightModeChangeEventArgs args)
        {
            NightModeEnabled = args.Enabled;
        }

        public async Task LoadDisplays(Dispatcher dispatcher)
        {
            Displays.Clear();
            
            List<MonitorInfo> saved = Util.Config.GetInstance().SavedDisplays;
            
            Loading = true;
            // when wrapped in Task, the Loader behaves correctly
            await Task.Run(async () =>
            {
                List<MonitorInfo> active = await ActiveDisplaysManager.MapDisplays(saved);
                await dispatcher.BeginInvoke(() =>
                {
                    foreach (var monitor in active)
                    {
                        Displays.Add(monitor);
                    }

                });
                Loading = false;
            });
        }

        public void SaveDisplays()
        {
            Util.Config.GetInstance().SaveDisplays(ManagedDisplays);
        }

        

        public async Task ChangeBrightness(Dispatcher dispatcher, bool night)
        {
            await dispatcher.BeginInvoke(() =>
            {
                foreach (MonitorInfo m in Displays)
                {
                    
                    m.SetBrightness(night ? ConfigType.NIGHT : ConfigType.DAY);    
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetCurrentToConfig(ConfigType t)
        {
            /*
            foreach(MonitorInfo m in ManagedDisplays)
            {
                if (m.DayConfig != null)
                {
                    m.DayConfig.Brightness = m.GetBrightness();
                }
            }
            */
            // it's required to do it this way so changes can be observed
            for(int i = 0; i < Displays.Count; i++)
            {
                MonitorInfo m = Displays[i].Clone();
                m.GetConfig(t).Brightness = m.GetBrightness();
                Displays[i] = m;
            }
        }
    }
}
