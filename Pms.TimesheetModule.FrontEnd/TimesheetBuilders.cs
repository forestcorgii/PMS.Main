using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Common;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using System;

namespace Pms.TimesheetModule.FrontEnd
{
    public static class TimesheetBuilders
    {
        public static ServiceCollection AddTimesheet(this ServiceCollection services, IConfigurationRoot conf, string connectionName)
        {
            string connectionString = conf.GetConnectionString(connectionName);
            services.AddSingleton<IDbContextFactory<TimesheetDbContext>>(new TimesheetDbContextFactory(connectionString));
            services.AddSingleton(TimeDownloaderFactory.CreateAdapter(conf));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<TimesheetDbContext> timesheetDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<TimesheetDbContext>>();
            using (TimesheetDbContext dbContext = timesheetDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();

            services.AddSingleton<Models.Timesheets>();

            services.AddSingleton<TimesheetProvider>();
            services.AddSingleton<IDownloadContentProvider, DownloadContentProvider>();
            services.AddSingleton<TimesheetManager>();

            services.AddSingleton<TimesheetListingVm>();
            services.AddSingleton<Func<TimesheetListingVm>>((s) => () => s.GetRequiredService<TimesheetListingVm>());
            services.AddSingleton<NavigationService<TimesheetListingVm>>();

            return services;
        }
    }
}
