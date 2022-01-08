using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface IManagerData
{
    Task<IEnumerable<Manager>> GetSinceAsync(DateTime fromDate, string connStrKey);
}