using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Common;
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

            services.AddSingleton<TimesheetViewModel>();
            services.AddSingleton<Func<TimesheetViewModel>>((s) => () => s.GetRequiredService<TimesheetViewModel>());
            services.AddSingleton<NavigationService<TimesheetViewModel>>();

            services.AddSingleton<MasterlistViewModel>();
            services.AddSingleton<Func<MasterlistViewModel>>((s) => () => s.GetRequiredService<MasterlistViewModel>());
            services.AddSingleton<NavigationService<MasterlistViewModel>>();

            services.AddSingleton<BillingViewModel>();
            services.AddSingleton<Func<BillingViewModel>>((s) => () => s.GetRequiredService<BillingViewModel>());
            services.AddSingleton<NavigationService<BillingViewModel>>();

            services.AddSingleton<PayrollViewModel>();
            services.AddSingleton<Func<PayrollViewModel>>((s) => () => s.GetRequiredService<PayrollViewModel>());
            services.AddSingleton<NavigationService<PayrollViewModel>>();

            services.AddSingleton<AlphalistViewModel>();
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
