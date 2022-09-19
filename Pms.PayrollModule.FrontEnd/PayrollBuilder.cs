using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Common;
using Pms.PayrollModule.FrontEnd.Models;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain.Services;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.ServiceLayer.EfCore;
using System;

namespace Pms.PayrollModule.FrontEnd
{
    public static class PayrollBuilder
    {
        public static ServiceCollection AddPayroll(this ServiceCollection services, IConfigurationRoot conf)
        {

            string connectionString = conf.GetConnectionString("Default");

            services.AddSingleton<IDbContextFactory<PayrollDbContext>>(new PayrollDbContextFactory(connectionString));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<PayrollDbContext> payrollDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<PayrollDbContext>>();
            using (PayrollDbContext dbContext = payrollDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            services.AddTransient<PayrollModel>();


            services.AddSingleton<IManagePayrollService, PayrollManager>();
            services.AddSingleton<IProvidePayrollService, PayrollProvider>();


            services.AddSingleton<AlphalistViewModel>();
            services.AddSingleton<Func<AlphalistViewModel>>((s) => () => s.GetRequiredService<AlphalistViewModel>());
            services.AddSingleton<NavigationService<AlphalistViewModel>>();

            services.AddSingleton<PayrollViewModel>();
            services.AddSingleton<Func<PayrollViewModel>>((s) => () => s.GetRequiredService<PayrollViewModel>());
            services.AddSingleton<NavigationService<PayrollViewModel>>();

            return services;
        }
    }
}
