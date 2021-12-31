using System;
using System.Threading;

namespace DesktopUI.Library
{
    public class QueryTimer : IQueryTimer
    {
        private Timer? _timer;

        public event QueryTimerElapsed? Elapsed;

        public IQueryTimer Start(int interval)
        {
            _timer = new Timer(Timer_Elapsed, null, 0, interval);
            return this;
        }

        public IQueryTimer ChangeInterval(int interval)
        {
            _timer?.Change(0, interval);
            return this;
        }

        public void Stop()
        {
            _timer?.Dispose();
        }

        private void Timer_Elapsed(object? state)
        {
            Elapsed?.Invoke(DateTime.Now);
        }
    }
}
