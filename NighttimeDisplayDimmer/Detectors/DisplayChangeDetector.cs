using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NighttimeDisplayDimmer.Detectors.NighttimeDetector;

namespace NighttimeDisplayDimmer.Detectors
{
    internal class DisplayChangeDetector : Detector
    {
        private static DisplayChangeDetector? singleton;

        public class DisplayChangeEventArgs : EventArgs
        {
            
        }

        public delegate void DisplayChangeEventHandler(object sender, DisplayChangeEventArgs args);
        public event DisplayChangeEventHandler? DisplaysChanged;

        private DisplayChangeDetector()
        {
        }

        public static DisplayChangeDetector GetInstance()
        {
            if (singleton == null)
            {
                singleton = new DisplayChangeDetector();
            }
            return singleton;
        }

        protected override void OnTick()
        {
            if(Monitorian.Core.Models.Monitor.MonitorManager.CheckMonitorsChanged())
            {
                DisplayChangeEventHandler? e = DisplaysChanged;
                e?.Invoke(this, new DisplayChangeEventArgs());
            }
        }
    }
}
