using Microsoft.Extensions.Configuration;

namespace DesktopUI.Library;

public class QueryTimerService
{
    private readonly IConfiguration _config;
    private int _queryInterval;

    public QueryTimerService(IConfiguration config)
    {
        _config = config;
        _queryInterval = config.GetValue<int>("DefaultQueryInterval");
        LogTimer = new QueryTimer();
        ReconciliationTimer = new QueryTimer();
        ManagerTimer = new QueryTimer();
    }

    public IQueryTimer LogTimer { get; }
    public IQueryTimer ReconciliationTimer { get; }
    public IQueryTimer ManagerTimer { get; }

    public void StartAll()
    {
        LogTimer.Start(_queryInterval);
        ReconciliationTimer.Start(_queryInterval);
        ManagerTimer.Start(_queryInterval);
    }

    public void StopAll()
    {
        LogTimer?.Stop();
        ReconciliationTimer?.Stop();
        ManagerTimer?.Stop();
    }
}