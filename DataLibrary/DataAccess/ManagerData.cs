using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Helpers;
using DataLibrary.Models;

namespace DataLibrary.DataAccess;

public class ManagerData : IManagerData
{
    private readonly IDataAccess _db;
    private readonly ManagerDataHelper _helper;

    public ManagerData(IDataAccess db, ManagerDataHelper helper)
    {
        _db = db;
        _helper = helper;
    }

    public async Task<List<Manager>> GetSinceAsync(DateTime fromDate, string connStrKey)
    {
        var engineProperties = (from e in await _db.GetEnginePropertiesAsync(connStrKey)
                                where e.TIMESTAMP > fromDate
                                orderby e.TIMESTAMP
                                select e).ToList();

        var modifiedManagers = _helper.GetModifiedManagers(engineProperties);

        return modifiedManagers;
    }
}