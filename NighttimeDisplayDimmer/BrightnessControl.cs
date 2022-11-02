using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace NighttimeDisplayDimmer
{
    // https://github.com/stpwin/MonitorBrightness/blob/master/MonitorBrightness/BrightnessControl.cs   
    class BrightnessControl
    {
        private static BrightnessControl? instance;

        public static BrightnessControl GetInstance()
        {
            if(instance == null)
            {
                instance = new BrightnessControl();
            }
            return instance;
        }

        private List<MonitorInfo> displays = new List<MonitorInfo>();

        public List<MonitorInfo> Displays { get => displays; }

        private BrightnessControl()
        {
            LoadDisplays();
        }

        public static uint monCount = 0;

        public void LoadDisplays()
        {
            displays.Clear();
#if DEBUG
            if (NativeCalls.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Callb, 0))

                Console.WriteLine("You have {0} monitors", monCount);
            else
                Console.WriteLine("An error occured while enumerating monitors");
#else
            NativeCalls.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Callb, 0);
#endif
            // https://github.com/Ericvf/DDC-CI/blob/master/DDCCI/DisplayService.cs
            foreach (var display in displays)
            {
                var info = new NativeStructures.MonitorInfoEx();
                NativeCalls.GetMonitorInfo(new HandleRef(null, display.Desktop), info);
                display.Name = new string(info.szDevice).TrimEnd('\0');
                display.Brightness = GetBrightnessCapabilities(display.Index);
            }
        }
        private bool Callb(IntPtr hMonitor, IntPtr hDC, ref NativeStructures.Rect prect, int d)
        {
            //monitors.Add(hMonitor);
            int lastWin32Error;
            uint pdwNumberOfPhysicalMonitors = 0;
            bool numberOfPhysicalMonitorsFromHmonitor = NativeCalls.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors);
            lastWin32Error = Marshal.GetLastWin32Error();

            var pPhysicalMonitorArray = new NativeStructures.PHYSICAL_MONITOR[pdwNumberOfPhysicalMonitors];
            bool physicalMonitorsFromHmonitor = NativeCalls.GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray);
            lastWin32Error = Marshal.GetLastWin32Error();

            displays.Add(new MonitorInfo() { Index=displays.Count, Desktop = hMonitor, Handle = hDC, Physical = pPhysicalMonitorArray[0].hPhysicalMonitor });

            //Console.WriteLine($"Handle: 0x{hMonitor:X}, Num: {pdwNumberOfPhysicalMonitors}, Physical: {pPhysicalMonitorArray[0].hPhysicalMonitor}");

            //GetMonitorCapabilities((int)monCount);
            return ++monCount > 0;
        }

        //private static void GetMonitorCapabilities(int monitorNumber)
        //{
        //    uint pdwMonitorCapabilities = 0u;
        //    uint pdwSupportedColorTemperatures = 0u;
        //    var monitorCapabilities = NativeCalls.GetMonitorCapabilities(monitors[monitorNumber], ref pdwMonitorCapabilities, ref pdwSupportedColorTemperatures);
        //    Debug.WriteLine(pdwMonitorCapabilities);
        //    Debug.WriteLine(pdwSupportedColorTemperatures);
        //    int lastWin32Error = Marshal.GetLastWin32Error();
        //    NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE type = NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE.MC_SHADOW_MASK_CATHODE_RAY_TUBE;
        //    var monitorTechnologyType = NativeCalls.GetMonitorTechnologyType(monitors[monitorNumber], ref type);
        //    Debug.WriteLine(type);
        //    lastWin32Error = Marshal.GetLastWin32Error();
        //}

        public bool SetBrightness(short brightness, int monitorNumber)
        {
            var brightnessWasSet = NativeCalls.SetMonitorBrightness(displays[monitorNumber].Physical, brightness);
            //if (brightnessWasSet)
            //    Debug.WriteLine("Brightness set to " + (short)brightness);
            int lastWin32Error = Marshal.GetLastWin32Error();
            return brightnessWasSet;
        }

        public BrightnessInfo GetBrightnessCapabilities(int monitorNumber)
        {
            short current = -1, minimum = -1, maximum = -1;
            bool getBrightness = NativeCalls.GetMonitorBrightness(displays[monitorNumber].Physical, ref minimum, ref current, ref maximum);
            int lastWin32Error = Marshal.GetLastWin32Error();
            return new BrightnessInfo { Minimum = minimum, Maximum = maximum, Current = current };
        }

        //public void DestroyMonitors(uint pdwNumberOfPhysicalMonitors, NativeStructures.PHYSICAL_MONITOR[] pPhysicalMonitorArray)
        //{
        //    var destroyPhysicalMonitors = NativeCalls.DestroyPhysicalMonitors(pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray);
        //    int lastWin32Error = Marshal.GetLastWin32Error();
        //}
    }
}