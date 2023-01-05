using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;
using Pms.MasterlistModule.FrontEnd;
using Pms.TimesheetModule.FrontEnd;
using Pms.PayrollModule.FrontEnd;
using Pms.Main.FrontEnd.PayrollApp.ViewModels;
using Pms.Main.FrontEnd.Common;
using Pms.AdjustmentModule.FrontEnd;

namespace Pms.Main.FrontEnd.PayrollApp
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
            IConfigurationRoot conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            bool isDevelopment = !true;
            string connectionName = isDevelopment ? "Development" : "Production";

            services
                .AddMasterlist(conf, connectionName)
                .AddTimesheet(conf, connectionName)
                .AddAdjustment(conf, connectionName)
                .AddPayroll(conf, connectionName);

            services.AddSingleton<NavigationStore>();


            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
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
