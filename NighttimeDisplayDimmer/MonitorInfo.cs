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
                DayConfig = DayConfig.Clone(),
                NightConfig = NightConfig.Clone(),
                Enabled = Enabled,
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is MonitorInfo info &&
                   DeviceInstanceId == info.DeviceInstanceId &&
                   Enabled == info.Enabled &&
                   EqualityComparer<BrightnessConfig?>.Default.Equals(DayConfig, info.DayConfig) &&
                   EqualityComparer<BrightnessConfig?>.Default.Equals(NightConfig, info.NightConfig);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceInstanceId, Enabled, DayConfig, NightConfig);
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
                return brightness != Brightness;
            }
            if(Type == ConfigType.NIGHT)
            {
                return brightness > Brightness;
            } else
            {
                return brightness < Brightness;
            }
            
        }

        public override bool Equals(object? obj)
        {
            return obj is BrightnessConfig config &&
                   Brightness == config.Brightness &&
                   Force == config.Force &&
                   Type == config.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Brightness, Force, Type);
        }

        public BrightnessConfig Clone()
        {
            return new BrightnessConfig { Brightness = Brightness, Force = Force, Type = Type };
        }
    }

    internal enum ConfigType
    {
        DAY,
        NIGHT
    }
}
