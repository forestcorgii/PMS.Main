using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Employees.Persistence;
using Pms.Employees.ServiceLayer.Concrete;
using Pms.Employees.ServiceLayer.EfCore;
using Pms.Employees.ServiceLayer.HRMS;
using Pms.Employees.ServiceLayer.HRMS.Adapter;
using Pms.Employees.ServiceLayer.HRMS.Service;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Services;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Timesheets.BizLogic;
using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.EfCore.Concrete;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

            IConfigurationRoot conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            string connectionString = conf.GetConnectionString("Default");

            services.AddSingleton(new TimesheetDbContextFactory(connectionString));
            services.AddSingleton(new EmployeeDbContextFactory(connectionString));

            services.AddSingleton(TimeDownloaderFactory.CreateAdapter(conf));
            services.AddSingleton(HRMSAdapterFactory.CreateAdapter(conf));

            services.AddSingleton<IEmployeeProvider, ListEmployeesService>();
            services.AddSingleton<IEmployeeSaving, SaveEmployeeService>();
            services.AddSingleton<IEmployeeFinder, FindEmployeeService>();
            services.AddTransient<EmployeeModel>();

            services.AddSingleton<ITimesheetProvider, TimesheetProvider>();
            services.AddSingleton<ITimesheetPageProvider, TimesheetPageProvider>();
            services.AddSingleton<IDownloadContentProvider, DownloadContentProvider>();
            services.AddSingleton<ITimesheetSaving, SaveTimesheetBizLogic>();
            services.AddTransient<Cutoff>();
            services.AddTransient<CutoffTimesheet>();


            services.AddSingleton<NavigationStore>();
            services.AddSingleton<MainStore>();
            services.AddSingleton<TimesheetStore>();
            services.AddSingleton<EmployeeStore>();


            services.AddTransient<TimesheetViewModel>();
            services.AddSingleton<Func<TimesheetViewModel>>((s) => () => s.GetRequiredService<TimesheetViewModel>());
            services.AddSingleton<NavigationService<TimesheetViewModel>>();

            services.AddTransient<EmployeeViewModel>();
            services.AddSingleton<Func<EmployeeViewModel>>((s) => () => s.GetRequiredService<EmployeeViewModel>());
            services.AddSingleton<NavigationService<EmployeeViewModel>>();



            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });


            return services.BuildServiceProvider();
        }

        public App()
        {
            Services = ConfigureServices();

            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            TimesheetDbContextFactory timesheetDbContextFactory = Services.GetRequiredService<TimesheetDbContextFactory>();
            using (TimesheetDbContext dbContext = timesheetDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            EmployeeDbContextFactory employeeDbContextFactory = Services.GetRequiredService<EmployeeDbContextFactory>();
            using (EmployeeDbContext dbContext = employeeDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();


            MainWindow = Services.GetRequiredService<MainWindow>();

            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
