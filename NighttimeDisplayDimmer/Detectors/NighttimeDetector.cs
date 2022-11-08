using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer.Detectors
{
    internal class NighttimeDetector : Detector 
    {
        private static NighttimeDetector? singleton;

        
        private bool? nightMode;

        public bool? NightMode { get => nightMode; }

        public class NightModeChangeEventArgs : EventArgs
        {
            public bool Enabled { get; set; }
        }

        public delegate void NightModeChangeEventHandler(object sender, NightModeChangeEventArgs args);
        public event NightModeChangeEventHandler? NightModeChanged;

        private NighttimeDetector()
        {
        }

        public static NighttimeDetector GetInstance()
        {
            if (singleton == null)
            {
                singleton = new NighttimeDetector();
            }
            return singleton;
        }

        protected override void OnTick()
        {
            bool? current = nightMode;
            nightMode = IsNightLightEnabled();
            if (!current.HasValue || nightMode != current)
            {
                NightModeChangeEventHandler? e = NightModeChanged;
                e?.Invoke(this, new NightModeChangeEventArgs() { Enabled = (bool)nightMode });
            }
        }

        private bool IsNightLightEnabled()
        {
            const string BlueLightReductionStateKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CloudStore\Store\DefaultAccount\Current\default$windows.data.bluelightreduction.bluelightreductionstate\windows.data.bluelightreduction.bluelightreductionstate";
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(BlueLightReductionStateKey))
            {
                var data = key?.GetValue("Data");
                if (data is null)
                    return false;
                var byteData = (byte[])data;
                return byteData.Length > 24 && byteData[23] == 0x10 && byteData[24] == 0x00;
            }
        }
    }
}
