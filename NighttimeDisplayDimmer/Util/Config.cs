using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NighttimeDisplayDimmer.Util
{
    internal class Config
    {
        //https://stackoverflow.com/questions/2434534/serialize-an-object-to-string
        public static T? DeserializeJSON<T>(string? toDeserialize)
        {
            if(toDeserialize == null) { throw new ArgumentNullException(nameof(toDeserialize)); }
            return JsonSerializer.Deserialize<T>(toDeserialize);
        }

        public static string SerializeJSON<T>(T toSerialize)
        {
            return JsonSerializer.Serialize(toSerialize);
        }

        public static int RefreshInterval
        {
            get { return Resources.Settings.Default.RefreshInterval; }
        }

        public static List<MonitorInfo> SavedDisplays
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

        public static void SaveDisplays(IEnumerable<MonitorInfo> displays)
        {
            System.Collections.Specialized.StringCollection dts = new System.Collections.Specialized.StringCollection();
            foreach (MonitorInfo d in displays)
            {
                dts.Add(SerializeJSON<MonitorInfo>(d));
            }
            Resources.Settings.Default.Displays = dts;
            Resources.Settings.Default.Save();
        }

    }
}
