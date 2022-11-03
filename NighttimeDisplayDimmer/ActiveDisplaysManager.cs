using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace NighttimeDisplayDimmer
{
    internal class ActiveDisplaysManager
    {
        public static async Task<List<MonitorInfo>> MapDisplays(List<MonitorInfo> saved)
        {
            List<MonitorInfo> displays = new List<MonitorInfo>();
            foreach (var m in await Monitorian.Core.Models.Monitor.MonitorManager.EnumerateMonitorsAsync())
            {
                m.UpdateBrightness();
                MonitorInfo? i = saved.Find(ci => ci.Assign(m));
                if (i == null)
                {
                    i = new MonitorInfo { Name = m.Description, DeviceInstanceId = m.DeviceInstanceId, Enabled = false, Monitor = m, DayConfig = new BrightnessConfig { Brightness = m.Brightness, Force = false, Type = ConfigType.DAY }, NightConfig = new BrightnessConfig { Brightness = m.Brightness, Force = false, Type = ConfigType.NIGHT } };
                }
                displays.Add(i);
            }
            return displays;
        }
    }
}
