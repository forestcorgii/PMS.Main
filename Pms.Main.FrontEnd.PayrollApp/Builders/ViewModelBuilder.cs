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
            services.AddSingleton<BillingViewModel>();
            services.AddSingleton<Func<BillingViewModel>>((s) => () => s.GetRequiredService<BillingViewModel>());
            services.AddSingleton<NavigationService<BillingViewModel>>();


            services.AddSingleton<MainViewModel>();
            services.AddSingleton(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            return services;
        }
    }
}
