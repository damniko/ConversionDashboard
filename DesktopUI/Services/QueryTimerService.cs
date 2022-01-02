using DesktopUI.Library;

namespace DesktopUI.Services
{
    public class QueryTimerService
    {
        public QueryTimerService()
        {
            LogTimer = new QueryTimer();
            ReconciliationTimer = new QueryTimer();
        }

        public IQueryTimer LogTimer { get; }
        public IQueryTimer ReconciliationTimer { get; }

        public void StartAll()
        {
            LogTimer.Start(1000); // TODO - add query intervals to settings
            ReconciliationTimer.Start(5000);
        }

        public void StopAll()
        {
            LogTimer?.Stop();
            ReconciliationTimer?.Stop();
        }
    }
}
