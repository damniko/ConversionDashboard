using DesktopUI.Library;

namespace DesktopUI.Services
{
    public class QueryTimerService
    {
        public QueryTimerService()
        {
            LogTimer = new QueryTimer();
        }

        public IQueryTimer LogTimer { get; }
        
        public void StartAll()
        {
            LogTimer.Start(5000); // TODO - add query intervals to settings
        }

        public void StopAll()
        {
            LogTimer?.Stop();
        }
    }
}
