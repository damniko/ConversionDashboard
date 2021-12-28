using DataLibrary.DataAccess;

var dao = new ManagerData();

var result = dao.GetManagersSinceDate(DateTime.Now.AddYears(-1), "Default");
Console.WriteLine();