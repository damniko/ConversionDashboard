using System;

namespace DesktopUI.Library
{
    public delegate void QueryTimerElapsed(DateTime date);

    public interface IQueryTimer
    {
        event QueryTimerElapsed? Elapsed;

        IQueryTimer ChangeInterval(int interval);
        IQueryTimer Start(int interval);
        void Stop();
    }
}