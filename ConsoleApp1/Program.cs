using DataLibrary.DataAccess;
using DataLibrary.Helpers;

var dao = new ManagerDAO(new ConfigHelper());

var result = dao.Get("Default", -1);
Console.WriteLine();