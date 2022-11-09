using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using NighttimeDisplayDimmer.Detectors;
using Windows.ApplicationModel.Resources.Core;
using Windows.Gaming.Input.ForceFeedback;

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
            Util.Config c = Util.Config.GetInstance();
            c.ConfigChanged += C_ConfigChanged;

            // set configured brightness on startup
            AdjustBrightness(NighttimeDetector.GetInstance().NightMode ?? false ? ConfigType.NIGHT : ConfigType.DAY);
        }

        private ConfigType GetConfigType() => NighttimeDetector.GetInstance().NightMode ?? false ? ConfigType.NIGHT : ConfigType.DAY;

        private void C_ConfigChanged(object sender, Util.Config.ConfigChangeEventArgs args)
        {
            AdjustBrightness(GetConfigType(), true);
        }

        private void D2_DisplaysChanged(object sender, DisplayChangeDetector.DisplayChangeEventArgs args)
        {
            AdjustBrightness(GetConfigType());
        }

        private void NightModeChanged(object sender, NighttimeDetector.NightModeChangeEventArgs args)
        {
            ConfigType t = args.Enabled ? ConfigType.NIGHT : ConfigType.DAY;
            AdjustBrightness(t);
        }

        private async void AdjustBrightness(ConfigType t, bool force = false)
        {
            List<MonitorInfo> list = await ActiveDisplaysManager.MapDisplays(Util.Config.GetInstance().SavedDisplays);
            bool changed = false;
            foreach (MonitorInfo info in list.Where(m => m.Present && m.Enabled && (m.Supported ?? false)))
            {
                if (info.IsConfigApplicable(t, info.GetBrightness(false)) || (force && info.GetBrightness(false) != info.GetConfig(t).Brightness))
                {
                    info.SetBrightness(t);
                    changed = true;
                }
            }
            if(changed && Util.Config.GetInstance().EnableNotifications)
            {
                
                new ToastContentBuilder()
                    .AddText(Properties.Localization.NotificationTitle)
                    .AddText(Properties.Localization.NotificationText)
                    .Show();
            }
        }
    }
}
