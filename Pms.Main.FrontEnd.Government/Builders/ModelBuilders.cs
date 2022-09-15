using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Domain.Models;
using Pms.Main.FrontEnd.Government.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Builders
{
    static class ModelBuilders
    {
        public static ServiceCollection AddModels(this ServiceCollection services)
        {
            services.AddTransient<Cutoff>();
            services.AddTransient<MonthlyPayrollModel>();
            services.AddTransient<EmployeeModel>();

            return services;
        }
    }
}
