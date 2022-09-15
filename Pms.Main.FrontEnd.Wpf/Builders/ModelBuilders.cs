using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Domain.Models;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Builders
{
    static class ModelBuilders
    {
        public static ServiceCollection AddModels(this ServiceCollection services)
        {
            services.AddTransient<Cutoff>();
            services.AddTransient<MasterlistModel>();
            services.AddTransient<TimesheetModel>();
            services.AddTransient<BillingModel>();
            services.AddTransient<PayrollModel>();

            return services;
        }
    }
}
