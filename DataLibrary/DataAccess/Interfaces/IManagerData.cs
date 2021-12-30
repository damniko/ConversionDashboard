using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IManagerData
    {
        IEnumerable<Manager> GetManagersSinceDate(DateTime fromDate, string connStrKey);
    }
}