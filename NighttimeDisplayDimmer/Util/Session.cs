using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer.Util
{
    internal class Session
    {
        public static bool IsRemote { get => Windows.System.RemoteDesktop.InteractiveSession.IsRemote; }
    }
}
