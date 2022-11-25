using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Monitorian.Core.Models.Monitor;
using NighttimeDisplayDimmer.Detectors;
using Windows.ApplicationModel.Resources.Core;
using Windows.Gaming.Input.ForceFeedback;

namespace NighttimeDisplayDimmer
{
    internal class BrightnessChanger
    {
        //private ILogger<BrightnessChanger> logger;
        public BrightnessChanger()
        {
            //logger = Util.Config.GetInstance().LogFactory.CreateLogger<BrightnessChanger>();
            
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
            if(Util.Session.IsRemote)
            {
                return;
            }
            List<MonitorInfo> list = await ActiveDisplaysManager.MapDisplays(Util.Config.GetInstance().SavedDisplays);

            // handle situation when a known monitor is listed as unreachable
            int retries = 10;
            while (list.Find(d => !(d.Supported ?? false)) != null && retries > 0)
            {
                list = await ActiveDisplaysManager.MapDisplays(Util.Config.GetInstance().SavedDisplays);
                retries -= 1;
                System.Threading.Thread.Sleep(1000);
            }

            bool changed = false;
            // check for support moved to ActiveDisplaysManager
            // in some cases, right after connecting the display, capabilities are not recognized correctly
            foreach (MonitorInfo info in list.Where(m => m.Present && m.Enabled/* && (m.Supported ?? false)*/))
            {
                if (info.IsConfigApplicable(t, info.GetBrightness(false)) || (force && info.GetBrightness(false) != info.GetConfig(t).Brightness))
                {
                    info.SetBrightness(t);
                    changed = true;
                }
            }
            if (changed)
            {
                //logger.LogInformation($"{Properties.Localization.NotificationText}\n\nMode {t}\n\nDisplays\n{String.Join('\n', list.Select(d => d.ToString()))}");
                if (Util.Config.GetInstance().EnableNotifications) {
                    new ToastContentBuilder()
                        .AddText(Properties.Localization.NotificationTitle)
                        .AddText(Properties.Localization.NotificationText)
                        .Show();
                }
            } else
            {
                //logger.LogDebug($"Display change was detected, but no display was commanded.\n\nMode {t}\n\n Displays\n{String.Join('\n', list.Select(d => d.ToString()))}");
            }

        }
    }
}
