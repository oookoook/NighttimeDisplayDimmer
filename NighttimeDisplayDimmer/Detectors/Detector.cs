using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NighttimeDisplayDimmer.Detectors
{
    
    internal abstract class Detector : IDisposable
    {
        protected CancellationTokenSource? tokenSource;
        protected CancellationToken token;
        protected Task? worker;
        protected bool running = false;

        public bool Running { get => running; }

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

        public void Initialize(int intervalSeconds)
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            var dueTime = TimeSpan.Zero;
            var interval = TimeSpan.FromSeconds(intervalSeconds);

            worker = RunPeriodicAsync(OnTick, dueTime, interval, token);
        }

        protected abstract void OnTick();

        public void Stop()
        {
            tokenSource?.Cancel();
        }

        public void Dispose()
        {
            tokenSource?.Dispose();
        }
    }
}
