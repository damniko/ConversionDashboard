using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Internal;
using DataLibrary.Models;
using DesktopUI.Controllers;
using DesktopUI.Library;
using DesktopUI.Models;
using DesktopUI.Services;
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
            Services = ConfigureServices();
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
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(typeof(App));

            services
                .AddSingleton<QueryTimerService>();

            services
                .AddTransient<IQueryTimer, QueryTimer>()
                .AddTransient<IDataAccess, EfDataAccess>()
                .AddTransient<IConfiguration, Config>()
                .AddTransient<ILogData, LogData>()
                .AddTransient<ControlBarViewModel>()
                .AddTransient<LogController>()
                .AddTransient<ReconciliationViewModel>()
                .AddTransient<LogViewModel>()
                .AddTransient<ShellViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
