using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.TimesheetApp.ViewModels;
using Pms.MasterlistModule.FrontEnd;
using Pms.TimesheetModule.FrontEnd;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.TimesheetApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App() =>
            Services = ConfigureServices();


        private static IServiceProvider ConfigureServices()
        {
            IConfigurationRoot conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            ServiceCollection services = new();
            bool isDevelopment = true;
            string connectionName = isDevelopment ? "Development" : "Production";
            services
                .AddMasterlist(conf, connectionName)
                .AddTimesheet(conf, connectionName);

            services.AddSingleton<NavigationStore>();


            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
                Title = $"Payroll Management System v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}",
                DataContext = s.GetRequiredService<MainViewModel>()
            });


            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
