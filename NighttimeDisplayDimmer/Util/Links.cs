using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer.Util
{
    internal class Links
    {

        public static readonly string Help = "https://sunsetdimmer.nastojte.cz/help.html";
        public static readonly string License = "https://sunsetdimmer.nastojte.cz/license.html";
        public static Task<bool> Open(string url)
        {
            return Windows.System.Launcher.LaunchUriAsync(new System.Uri(url)).AsTask();
        }


    }
}
