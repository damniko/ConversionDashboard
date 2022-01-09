using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;
using DataLibrary.Models.Database;

namespace DataLibrary.DataAccess;

public class AltManagerData : IManagerData
{
    private readonly IDataAccess _db;

    public AltManagerData(IDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<ENGINE_PROPERTY>> GetEnginePropsSince(DateTime fromDate, string connStrKey)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Manager>> GetSinceAsync(DateTime fromDate, string connStrKey)
    {
        var data = await _db.GetEnginePropertiesAsync(connStrKey);
        return new List<Manager>();
    }
}
