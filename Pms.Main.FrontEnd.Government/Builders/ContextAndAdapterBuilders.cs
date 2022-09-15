using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Persistence;
using Pms.Employees.Persistence;
using Pms.Payrolls.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Builders
{
    static class ContextAndAdapterBuilders
    {
        public static ServiceCollection AddContextAndAdapter(this ServiceCollection services)
        {
            IConfigurationRoot conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            string connectionString = conf.GetConnectionString("Default");

            services.AddSingleton<IDbContextFactory<EmployeeDbContext>>(new EmployeeDbContextFactory(connectionString));
            services.AddSingleton<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));
            services.AddSingleton<IDbContextFactory<PayrollDbContext>>(new PayrollDbContextFactory(connectionString));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<PayrollDbContext> payrollDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<PayrollDbContext>>();
            using (PayrollDbContext dbContext = payrollDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            IDbContextFactory<EmployeeDbContext> employeeDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<EmployeeDbContext>>();
            using (EmployeeDbContext dbContext = employeeDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            IDbContextFactory<AdjustmentDbContext> adjustmentDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<AdjustmentDbContext>>();
            using (AdjustmentDbContext dbContext = adjustmentDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();




            return services;
        }
    }
}
