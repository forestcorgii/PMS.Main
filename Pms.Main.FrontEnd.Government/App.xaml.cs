using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Government.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Government
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
                .AddStores()
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
