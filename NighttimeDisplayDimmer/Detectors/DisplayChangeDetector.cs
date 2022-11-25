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

        private int ticksDelay = -1;
        private bool remoteLast;

        private DisplayChangeDetector()
        {
            remoteLast = Util.Session.IsRemote;
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
            bool remoteNow = Util.Session.IsRemote;
            if (!remoteNow && (Monitorian.Core.Models.Monitor.MonitorManager.CheckMonitorsChanged() || remoteLast))
            {
                // little cooldown so windows have time to process the new display...
                //System.Threading.Thread.Sleep(5000);
                ticksDelay = 2;
            }

            if(ticksDelay == 0)
            {
                DisplayChangeEventHandler? e = DisplaysChanged;
                e?.Invoke(this, new DisplayChangeEventArgs());
            }
            if(ticksDelay >= 0)
            {
                ticksDelay -= 1;
            }
            remoteLast = remoteNow;
        }
    }
}
