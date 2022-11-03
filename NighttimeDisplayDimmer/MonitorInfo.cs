using Monitorian.Core.Models.Monitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer
{
    internal class MonitorInfo
    {
        public string? Name { get; set; }
        public string? DeviceInstanceId { get; set; }
        [JsonIgnore]
        public IMonitor? Monitor { get; set; }
        public bool Enabled { get; set; }
        public bool Present { get { return Monitor != null; } }
        public bool? Supported { get { return Monitor?.IsBrightnessSupported; } }
        public BrightnessConfig? DayConfig { get; set; }
        public BrightnessConfig? NightConfig { get; set; }

        public bool Assign(IMonitor m)
        {
            if(m.DeviceInstanceId == DeviceInstanceId)
            {
                Monitor = m;
                return true;
            }
            return false;
        }


        public int GetBrightness(bool refresh = true)
        {
            if (Monitor == null) { throw new InvalidOperationException("Monitor not found"); }
            if (refresh)
            {
                Monitor.UpdateBrightness();
            }
            return Monitor.Brightness;
        }

        public void SetBrightness(ConfigType t)
        {
            ChangeBrightness(GetConfig(t));
        }

        public void ChangeBrightness(BrightnessConfig? c)
        {
            if(c == null) { throw new ArgumentNullException(nameof(c)); }
            if(Monitor == null) { throw new InvalidOperationException("Monitor not found"); }
            Monitor.SetBrightness(c.Brightness);
        }

        public BrightnessConfig GetConfig(ConfigType t)
        {
            if(NightConfig == null || DayConfig == null)
            {
                throw new InvalidOperationException("Config not set");
            }
            switch(t)
            {
                case ConfigType.NIGHT: return NightConfig;
                default: return DayConfig;
            }
        }

        public bool IsConfigApplicable(ConfigType t, int brightness)
        {
            return GetConfig(t).IsApplicable(brightness);
        }
        
        public MonitorInfo Clone()
        {
            return new MonitorInfo
            {
                Name = Name,
                DeviceInstanceId = DeviceInstanceId,
                Monitor = Monitor,
                DayConfig = DayConfig,
                NightConfig = NightConfig,
                Enabled = Enabled,
            };
        }
    }

    internal class BrightnessConfig
    {
        public int Brightness { get; set; }
        public bool Force { get; set; }
        public ConfigType Type { get; set; }

        public bool IsApplicable(int brightness)
        {
            if(Force)
            {
                return true;
            }
            if(Type == ConfigType.NIGHT)
            {
                return brightness > Brightness;
            } else
            {
                return brightness < Brightness;
            }
            
        }
    }

    internal enum ConfigType
    {
        DAY,
        NIGHT
    }
}
