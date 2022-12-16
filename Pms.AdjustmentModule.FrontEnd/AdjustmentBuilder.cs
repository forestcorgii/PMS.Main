using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.ServiceLayer.EfCore.Billing_Records;
using Pms.Adjustments.ServiceLayer.EfCore.Table_Views;
using Pms.Adjustments.ServiceLayer.Files;
using Pms.Main.FrontEnd.Common;
using System;

namespace Pms.AdjustmentModule.FrontEnd
{
    public static class AdjustmentBuilder
    {
        public static ServiceCollection AddAdjustment(this ServiceCollection services, IConfigurationRoot conf, string connectionName)
        {
            string connectionString = conf.GetConnectionString(connectionName);
            
            services.AddSingleton<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<AdjustmentDbContext> adjustmentDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<AdjustmentDbContext>>();
            using (AdjustmentDbContext dbContext = adjustmentDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            services.AddTransient<Billings>();
            services.AddTransient<BillingRecords>();
            services.AddTransient<Employees>();

            services.AddSingleton<BillingListingVm>();
            services.AddSingleton<Func<BillingListingVm>>((s) => () => s.GetRequiredService<BillingListingVm>());
            services.AddSingleton<NavigationService<BillingListingVm>>();

            services.AddSingleton<BillingRecordListingVm>();
            services.AddSingleton<Func<BillingRecordListingVm>>((s) => () => s.GetRequiredService<BillingRecordListingVm>());
            services.AddSingleton<NavigationService<BillingRecordListingVm>>();

            services.AddSingleton<IManageBillingService, BillingManager>();
            services.AddSingleton<IProvideBillingService, BillingProvider>();
            services.AddSingleton<IGenerateBillingService, BillingGenerator>();
            services.AddSingleton<BillingExporter>();

            services.AddSingleton<BillingRecordImporter>();
            services.AddSingleton<BillingRecordProvider>();
            services.AddSingleton<EmployeeViewProvider>();
            services.AddSingleton<BillingRecordManager>();


            return services;
        }
    }
}
