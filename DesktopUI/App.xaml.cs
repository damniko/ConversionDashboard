using System;
using System.Windows;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DesktopUI.Controllers;
using DesktopUI.Library;
using DesktopUI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DesktopUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Configuration = GetConfiguration();
            Services = ConfigureServices();
        }

        public IConfiguration Configuration { get; }

        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json");
#if DEBUG
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
            builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif
            return builder.Build();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use.
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(typeof(App));

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Debug"))
                    .AddDebug();   
            });

            services
                .AddSingleton(provider => GetConfiguration())
                .AddSingleton<QueryTimerService>()
                .AddTransient<IQueryTimer, QueryTimer>()
                .AddTransient<IDataAccess, EfDataAccess>()
                .AddTransient<ILogData, LogData>()
                .AddTransient<IReconciliationData, ReconciliationData>()
                .AddTransient<ControlBarViewModel>()
                .AddTransient<LogController>()
                .AddTransient<ReconciliationController>()
                .AddTransient<ReconciliationViewModel>()
                .AddTransient<LogViewModel>()
                .AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
