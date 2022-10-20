using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer.HRMS.Services;
using Pms.Payrolls.Domain.Services;
using Pms.Payrolls.ServiceLayer.EfCore;
using Pms.Payrolls.ServiceLayer.Files;
using Pms.Payrolls.ServiceLayer.Files.Exports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Builders
{
    static class ServiceBuilders
    {
        public static ServiceCollection AddServices(this ServiceCollection services)
        {
            services.AddSingleton<CompanyManager>();
            services.AddSingleton<PayrollCodeManager>();

            services.AddSingleton<EmployeeProvider>();
            services.AddSingleton<EmployeeManager>();
            services.AddSingleton<HrmsEmployeeProvider>();
            services.AddSingleton<EmployeeBankInformationImporter>();

            services.AddSingleton<IManageBillingService, BillingManager>();
            services.AddSingleton<IProvideBillingService, BillingProvider>();
            services.AddSingleton<IGenerateBillingService, BillingGenerator>();
            services.AddSingleton<BillingExporter>();

            services.AddSingleton<PayrollManager>();
            services.AddSingleton<PayrollProvider>();


            return services;
        }
    }
}
