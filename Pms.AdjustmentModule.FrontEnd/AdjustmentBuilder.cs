using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.ServiceLayer.Files;
using Pms.Main.FrontEnd.Common;
using System;

namespace Pms.AdjustmentModule.FrontEnd
{
    public static class AdjustmentBuilder
    {
        public static ServiceCollection AddAdjustment(this ServiceCollection services, IConfigurationRoot conf)
        {
            string connectionString = conf.GetConnectionString("Default");
            
            services.AddSingleton<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<AdjustmentDbContext> adjustmentDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<AdjustmentDbContext>>();
            using (AdjustmentDbContext dbContext = adjustmentDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            services.AddTransient<Billings>();

            services.AddSingleton<BillingListingVm>();
            services.AddSingleton<Func<BillingListingVm>>((s) => () => s.GetRequiredService<BillingListingVm>());
            services.AddSingleton<NavigationService<BillingListingVm>>();

            services.AddSingleton<IManageBillingService, BillingManager>();
            services.AddSingleton<IProvideBillingService, BillingProvider>();
            services.AddSingleton<IGenerateBillingService, BillingGenerator>();
            services.AddSingleton<BillingExporter>();


            return services;
        }
    }
}
