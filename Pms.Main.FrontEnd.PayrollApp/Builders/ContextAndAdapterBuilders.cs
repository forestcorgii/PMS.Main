using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Persistence;
using Pms.MasterlistModule.FrontEnd;
using Pms.PayrollModule.FrontEnd;
using Pms.Payrolls.Persistence;
using Pms.TimesheetModule.FrontEnd;
using System;

namespace Pms.Main.FrontEnd.Wpf.Builders
{
    static class ContextAndAdapterBuilders
    {
        public static ServiceCollection AddContextAndAdapter(this ServiceCollection services)
        {
            IConfigurationRoot conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            string connectionString = conf.GetConnectionString("Default");

            services.AddMasterlist(conf);
            services.AddTimesheet(conf);
            services.AddPayroll(conf);

            services.AddSingleton<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));

            IServiceProvider serviceProvider = services.BuildServiceProvider();
             
            IDbContextFactory<AdjustmentDbContext> adjustmentDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<AdjustmentDbContext>>();
            using (AdjustmentDbContext dbContext = adjustmentDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();



            return services;
        }
    }
}
