using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Persistence;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.HRMS.Adapter;
using Pms.Payrolls.Persistence;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Builders
{
    static class ContextAndAdapterBuilders
    {
        public static ServiceCollection AddContextAndAdapter(this ServiceCollection services)
        {
            IConfigurationRoot conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            string connectionString = conf.GetConnectionString("Default");

            services.AddSingleton<IDbContextFactory<TimesheetDbContext>>(new TimesheetDbContextFactory(connectionString));
            services.AddSingleton<IDbContextFactory<EmployeeDbContext>>(new EmployeeDbContextFactory(connectionString));
            services.AddSingleton<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));
            services.AddSingleton<IDbContextFactory<PayrollDbContext>>(new PayrollDbContextFactory(connectionString));

            services.AddSingleton(TimeDownloaderFactory.CreateAdapter(conf));
            services.AddSingleton(HRMSAdapterFactory.CreateAdapter(conf));


            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<PayrollDbContext> payrollDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<PayrollDbContext>>();
            using (PayrollDbContext dbContext = payrollDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            IDbContextFactory<TimesheetDbContext> timesheetDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<TimesheetDbContext>>();
            using (TimesheetDbContext dbContext = timesheetDbContextFactory.CreateDbContext())
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
