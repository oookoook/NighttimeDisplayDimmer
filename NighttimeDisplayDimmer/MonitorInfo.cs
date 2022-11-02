using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer
{
    internal class MonitorInfo
    {
        public int Index { get; set; }
        public IntPtr Desktop { get; set; }
        public IntPtr Handle { get; set; }
        public IntPtr Physical { get; set; }
        public string? Name { get; set; }
        public BrightnessInfo? Brightness { get; set; }

        public MonitorInfo UpdateBrightness(BrightnessInfo brightness)
        {
            return new MonitorInfo()
            {
                Index = this.Index,
                Desktop = this.Desktop,
                Handle = this.Handle,
                Physical = this.Physical,
                Name = this.Name,
                Brightness = brightness
            };
        }
    }
}
