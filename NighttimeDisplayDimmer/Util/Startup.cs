using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace NighttimeDisplayDimmer.Util
{
    internal class Startup
    {
        private static string taskId = "NighttimeDisplayDimmer";
        public static bool Automatic
        {
            get
            {
                return GetStartupTask().State == StartupTaskState.Enabled;
            }
            internal set {
                if (!Allowed)
                {
                    throw new InvalidOperationException("Setting automatic startup is prohibited by user or policy");
                }
                StartupTask t = GetStartupTask();
                if (value && t.State == StartupTaskState.Enabled)
                {
                    return;
                } 
                else if (!value && t.State == StartupTaskState.Disabled)
                {
                    return;
                }
                else if (value)
                {
                    StartupTaskState result = t.RequestEnableAsync().AsTask().Result;
                    if (result != StartupTaskState.Enabled)
                    {
                        throw new InvalidOperationException("Unable to enable login on startup");
                    }
                } 
                else
                {
                    t.Disable();
                }
            }
        }

        public static bool Allowed
        {
            get {
                StartupTask t = GetStartupTask();
                return !(t.State == StartupTaskState.DisabledByUser || t.State == StartupTaskState.DisabledByPolicy);
            }
        }

        private static StartupTask GetStartupTask()
        {
            return StartupTask.GetAsync(taskId).AsTask().Result;
        }
    }
}