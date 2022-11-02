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
        public ObservableCollection<IMonitor> Displays { get; }
        public bool? NightModeEnabled { get => nightModeEnabled; set { nightModeEnabled = value; NotifyPropertyChanged(); } }

        private bool loading = false;
        public bool Loading { get => loading; set { loading = value; NotifyPropertyChanged(); } }

        public OptionsViewModel()
        {
            NighttimeDetector d = NighttimeDetector.GetInstance();
            d.NightModeChanged += NightModeChanged;
            NightModeEnabled = d.NightMode;
            Displays = new ObservableCollection<IMonitor>();
        }

        private void NightModeChanged(object sender, NighttimeDetector.NightModeChangeEventArgs args)
        {
            NightModeEnabled = args.Enabled;
        }

        public async Task LoadDisplays(Dispatcher dispatcher)
        {
            Displays.Clear();
            await Task.Run(async () =>
            {
                Loading = true;
                foreach (var m in await Monitorian.Core.Models.Monitor.MonitorManager.EnumerateMonitorsAsync())
                {
                    m.UpdateBrightness();
                    await dispatcher.BeginInvoke(() =>
                    {
                        Displays.Add(m);
                    });
                }
                Loading = false;
            });

        }

        public async Task ChangeBrightness(Dispatcher dispatcher)
        {
            await dispatcher.BeginInvoke(() =>
            {
                foreach (IMonitor m in Displays)
                {
                    m.SetBrightness(60);    
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
