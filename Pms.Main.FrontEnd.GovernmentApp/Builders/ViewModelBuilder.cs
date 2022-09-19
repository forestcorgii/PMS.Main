using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Government.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Builders
{
    static class ViewModelBuilder
    {
        public static ServiceCollection AddViewModels(this ServiceCollection services)
        {

            //services.AddTransient<EmployeeViewModel>();
            //services.AddSingleton<Func<EmployeeViewModel>>((s) => () => s.GetRequiredService<EmployeeViewModel>());
            //services.AddSingleton<NavigationService<EmployeeViewModel>>();

            //services.AddTransient<BillingViewModel>();
            //services.AddSingleton<Func<BillingViewModel>>((s) => () => s.GetRequiredService<BillingViewModel>());
            //services.AddSingleton<NavigationService<BillingViewModel>>();

            services.AddTransient<PayrollListingViewModel>();
            services.AddSingleton<Func<PayrollListingViewModel>>((s) => () => s.GetRequiredService<PayrollListingViewModel>());
            services.AddSingleton<NavigationService<PayrollListingViewModel>>();

            services.AddTransient<PayrollDetailViewModel>();
            services.AddSingleton<Func<PayrollDetailViewModel>>((s) => () => s.GetRequiredService<PayrollDetailViewModel>());
            services.AddSingleton<NavigationService<PayrollDetailViewModel>>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            return services;
        }
    }
}
