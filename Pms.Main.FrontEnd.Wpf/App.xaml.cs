using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Persistence;
using Pms.Masterlists.Persistence;
using Pms.Main.FrontEnd.Wpf.Builders;
using Pms.Timesheets.Persistence;
using System;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();
            services
                .AddContextAndAdapter()
                .AddServices()
                .AddModels()
                .AddViewModels();

            return services.BuildServiceProvider();
        }

        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {


            MainWindow = Services.GetRequiredService<MainWindow>();

            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
