using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Wpf.Services;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Builders
{
    static class ViewModelBuilder
    {
        public static ServiceCollection AddViewModels(this ServiceCollection services)
        {

            services.AddTransient<TimesheetViewModel>();
            services.AddSingleton<Func<TimesheetViewModel>>((s) => () => s.GetRequiredService<TimesheetViewModel>());
            services.AddSingleton<NavigationService<TimesheetViewModel>>();

            services.AddTransient<EmployeeViewModel>();
            services.AddSingleton<Func<EmployeeViewModel>>((s) => () => s.GetRequiredService<EmployeeViewModel>());
            services.AddSingleton<NavigationService<EmployeeViewModel>>();

            services.AddTransient<BillingViewModel>();
            services.AddSingleton<Func<BillingViewModel>>((s) => () => s.GetRequiredService<BillingViewModel>());
            services.AddSingleton<NavigationService<BillingViewModel>>();

            services.AddTransient<PayrollViewModel>();
            services.AddSingleton<Func<PayrollViewModel>>((s) => () => s.GetRequiredService<PayrollViewModel>());
            services.AddSingleton<NavigationService<PayrollViewModel>>();

            services.AddTransient<AlphalistViewModel>();
            services.AddSingleton<Func<AlphalistViewModel>>((s) => () => s.GetRequiredService<AlphalistViewModel>());
            services.AddSingleton<NavigationService<AlphalistViewModel>>();


            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            return services;
        }
    }
}
