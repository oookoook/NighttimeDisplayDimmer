using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer.Util
{
    internal class Links
    {

        public static readonly string Help = "https://oookoook.github.io/NighttimeDisplayDimmer/help.html";
        public static readonly string License = "https://oookoook.github.io/NighttimeDisplayDimmer/license.html";
        public static Task<bool> Open(string url)
        {
            return Windows.System.Launcher.LaunchUriAsync(new System.Uri(url)).AsTask();
        }


    }
}
