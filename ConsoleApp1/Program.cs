using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Internal;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

IDataAccess dataAccess = new EfDataAccess(config);

IReconciliationData reconciliationData = new ReconciliationData(dataAccess);

var result = reconciliationData.GetReconciliationsSinceDate(DateTime.Now.AddYears(-1), "Default");

Console.WriteLine();