using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Monitorian.Core.Models.Monitor;
using Windows.Storage.Search;

namespace NighttimeDisplayDimmer
{
    internal class OptionsViewModel : INotifyPropertyChanged
    {
        private bool? nightModeEnabled;
        public ObservableCollection<MonitorInfo> Displays { get; }
        public bool? NightModeEnabled { get => nightModeEnabled; set { nightModeEnabled = value; NotifyPropertyChanged(); } }

        private bool loading = false;
        public bool Loading { get => loading; set { loading = value; NotifyPropertyChanged(); } }

        public IEnumerable<MonitorInfo> ManagedDisplays { get => Displays.Where(d => d.Enabled && (d.Supported ?? false)); }

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
            
            List<MonitorInfo> saved = Util.Config.SavedDisplays;
            
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
            /*
            await Task.Run(async () =>
            {
                Loading = true;
                foreach (var m in await Monitorian.Core.Models.Monitor.MonitorManager.EnumerateMonitorsAsync())
                {
                    MonitorInfo? i = saved.Find(ci => ci.Assign(m));
                    if (i == null)
                    {
                        i = new MonitorInfo { Name = m.Description, DeviceInstanceId = m.DeviceInstanceId, Enabled = false, Monitor = m, DayConfig = new BrightnessConfig { Brightness = m.Brightness, Force = false }, NightConfig = new BrightnessConfig { Brightness = m.Brightness, Force = false } };
                    }
                    await dispatcher.BeginInvoke(() =>
                    {
                        Displays.Add(i);
                    });
                }
                Loading = false;
            });
            */
        }

        public void SaveDisplays()
        {
            Util.Config.SaveDisplays(ManagedDisplays);
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
