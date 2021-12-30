using ConsoleApp1;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

IDataAccess dataAccess = new EfDataAccess(config, new ConsoleLogger<EfDataAccess>());

dataAccess.GetManagersTbl("NoManagersTable");

IReconciliationData reconciliationData = new ReconciliationData(dataAccess);

var result = reconciliationData.GetReconciliationsSinceDate(DateTime.Now.AddYears(-1), "Default");

Console.WriteLine();