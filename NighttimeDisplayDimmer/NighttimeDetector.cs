using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer
{
    internal class NighttimeDetector : IDisposable
    {
        private static NighttimeDetector? singleton;
        
        private CancellationTokenSource? tokenSource;
        private CancellationToken token;
        private Task? worker;
        private bool running = false;
        private bool? nightMode;


        public bool Running { get => running; }
        public bool? NightMode { get => nightMode; }
        
        public class NightModeChangeEventArgs : EventArgs
        {
            public bool Enabled { get; set; }
        }

        public delegate void NightModeChangeEventHandler(object sender, NightModeChangeEventArgs args);
        public event NightModeChangeEventHandler? NightModeChanged;

        // https://stackoverflow.com/questions/14296644/how-to-execute-a-method-periodically-from-wpf-client-application-using-threading
        // The `onTick` method will be called periodically unless cancelled.
        private async Task RunPeriodicAsync(Action onTick,
                                                   TimeSpan dueTime,
                                                   TimeSpan interval,
                                                   CancellationToken token)
        {
            running = true;
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
            running = false;
        }

        private NighttimeDetector()
        {
        }

        public static NighttimeDetector GetInstance()
        {
            if(singleton == null)
            {
                singleton = new NighttimeDetector();
            }
            return singleton;
        }

        public void Initialize(int intervalSeconds)
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            var dueTime = TimeSpan.Zero;
            var interval = TimeSpan.FromSeconds(intervalSeconds);

            worker = RunPeriodicAsync(OnTick, dueTime, interval, token);
        }

        public void Stop()
        {
            tokenSource?.Cancel();
        }

        public void Dispose()
        {
            tokenSource?.Dispose();
        }

        private void OnTick()
        {
            bool? current = nightMode;
            nightMode = IsNightLightEnabled();
            if(!current.HasValue || nightMode != current)
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
