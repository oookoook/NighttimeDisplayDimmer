using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer
{
    internal class BrightnessChanger
    {
        public BrightnessChanger()
        {
            NighttimeDetector d = NighttimeDetector.GetInstance();
            d.NightModeChanged += NightModeChanged;
        }

        private async void NightModeChanged(object sender, NighttimeDetector.NightModeChangeEventArgs args)
        {
            List<MonitorInfo> list = await ActiveDisplaysManager.MapDisplays(Util.Config.SavedDisplays);
            ConfigType t = args.Enabled ? ConfigType.NIGHT : ConfigType.DAY;
            foreach (MonitorInfo info in list.Where(m => m.Present && m.Enabled && (m.Supported ?? false)))
            {
                if(info.IsConfigApplicable(t, info.GetBrightness(false)))
                {
                    info.SetBrightness(t);
                }
            }
        }
    }
}
