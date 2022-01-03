using Microsoft.Extensions.Configuration;

namespace DesktopUI.Library
{
    public class QueryTimerService
    {
        private int _queryInterval;

        public QueryTimerService(IConfiguration config)
        {
            LogTimer = new QueryTimer();
            ReconciliationTimer = new QueryTimer();
            _queryInterval = config.GetValue<int>("DefaultQueryInterval");
        }

        public IQueryTimer LogTimer { get; }
        public IQueryTimer ReconciliationTimer { get; }

        public void StartAll()
        {
            LogTimer.Start(_queryInterval);
            ReconciliationTimer.Start(_queryInterval);
        }

        public void StopAll()
        {
            LogTimer?.Stop();
            ReconciliationTimer?.Stop();
        }
    }
}
