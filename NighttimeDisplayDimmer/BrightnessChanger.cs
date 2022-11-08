using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NighttimeDisplayDimmer.Detectors;

namespace NighttimeDisplayDimmer
{
    internal class BrightnessChanger
    {
        public BrightnessChanger()
        {
            NighttimeDetector d = NighttimeDetector.GetInstance();
            d.NightModeChanged += NightModeChanged;
            DisplayChangeDetector d2 = DisplayChangeDetector.GetInstance();
            d2.DisplaysChanged += D2_DisplaysChanged;
        }

        private void D2_DisplaysChanged(object sender, DisplayChangeDetector.DisplayChangeEventArgs args)
        {
            AdjustBrightness(NighttimeDetector.GetInstance().NightMode ?? false ? ConfigType.NIGHT : ConfigType.DAY);
        }

        private void NightModeChanged(object sender, NighttimeDetector.NightModeChangeEventArgs args)
        {
            ConfigType t = args.Enabled ? ConfigType.NIGHT : ConfigType.DAY;
            AdjustBrightness(t);
        }

        private async void AdjustBrightness(ConfigType t)
        {
            List<MonitorInfo> list = await ActiveDisplaysManager.MapDisplays(Util.Config.SavedDisplays);
            foreach (MonitorInfo info in list.Where(m => m.Present && m.Enabled && (m.Supported ?? false)))
            {
                if (info.IsConfigApplicable(t, info.GetBrightness(false)))
                {
                    info.SetBrightness(t);
                }
            }
        }
    }
}
