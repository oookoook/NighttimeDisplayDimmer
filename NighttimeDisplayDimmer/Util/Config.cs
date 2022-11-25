//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.EventLog;
using NighttimeDisplayDimmer.Detectors;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Storage.Search;
using static NighttimeDisplayDimmer.Detectors.NighttimeDetector;

namespace NighttimeDisplayDimmer.Util
{
    internal class Config : IDisposable
    {
        public int RefreshInterval
        {
            get { return Resources.Settings.Default.RefreshInterval; }
        }

        public bool EnableNotifications
        {
            get => Resources.Settings.Default.EnableNotifications;
            set
            {
                Resources.Settings.Default.EnableNotifications = value;
                Resources.Settings.Default.Save();
            }
        }
        public List<MonitorInfo> SavedDisplays
        {
            get
            {
                List<MonitorInfo> output = new List<MonitorInfo>();
                System.Collections.Specialized.StringCollection savedDisplays = Resources.Settings.Default.Displays;
                if (savedDisplays == null)
                {
                    return output;
                }
                for (int i = 0; i < savedDisplays.Count; i++)
                {
                    MonitorInfo? m = DeserializeJSON<MonitorInfo>(savedDisplays[i]);
                    if (m != null)
                    {
                        output.Add(m);
                    }
                }
                return output;
            }
        }

        private static Config? singleton;

        public class ConfigChangeEventArgs : EventArgs
        {

        }

        public delegate void ConfigChangeEventHandler(object sender, ConfigChangeEventArgs args);
        public event ConfigChangeEventHandler? ConfigChanged;

        //public ILoggerFactory LogFactory { get; set; }

        private Config()
        {
            /*
            LogFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("NighttimeDisplayDimmer",
#if DEBUG
                    LogLevel.Debug
#else
                    LogLevel.Information
#endif
                    )
                    .AddEventLog(configuration => {
                        //configuration.LogName = "Application";
                        // this is not registered as a source
                        //configuration.SourceName = "Nighttime Display Dimmer";
                    });
            });
            */
        }

        public static Config GetInstance()
        {
            if (singleton == null)
            {
                singleton = new Config();
            }
            return singleton;
        }

        //https://stackoverflow.com/questions/2434534/serialize-an-object-to-string
        public T? DeserializeJSON<T>(string? toDeserialize)
        {
            if(toDeserialize == null) { throw new ArgumentNullException(nameof(toDeserialize)); }
            return JsonSerializer.Deserialize<T>(toDeserialize);
        }

        public string SerializeJSON<T>(T toSerialize)
        {
            return JsonSerializer.Serialize(toSerialize);
        }

        public void SaveDisplays(IEnumerable<MonitorInfo> displays)
        {
            System.Collections.Specialized.StringCollection dts = new System.Collections.Specialized.StringCollection();
            foreach (MonitorInfo d in displays)
            {
                dts.Add(SerializeJSON<MonitorInfo>(d));
            }
            Resources.Settings.Default.Displays = dts;
            Resources.Settings.Default.Save();
            ConfigChangeEventHandler? e = ConfigChanged;
            e?.Invoke(this, new ConfigChangeEventArgs());
        }

        public void Dispose()
        {
            //LogFactory.Dispose();
        }
    }
}
