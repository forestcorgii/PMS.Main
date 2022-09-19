using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pms.Main.FrontEnd.Common;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.ViewModels;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer.HRMS.Adapter;
using Pms.Masterlists.ServiceLayer.HRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.FrontEnd
{
    public static class MasterlistBuilder
    {
        public static ServiceCollection AddMasterlist(this ServiceCollection services, IConfigurationRoot conf)
        {
            string connectionString = conf.GetConnectionString("Default");
            services.AddSingleton<IDbContextFactory<EmployeeDbContext>>(new EmployeeDbContextFactory(connectionString));
            services.AddSingleton(HRMSAdapterFactory.CreateAdapter(conf));

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IDbContextFactory<EmployeeDbContext> employeeDbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<EmployeeDbContext>>();
            using (EmployeeDbContext dbContext = employeeDbContextFactory.CreateDbContext())
                dbContext.Database.Migrate();


            services.AddTransient<Employees>();

            services.AddSingleton<EmployeeProvider>();
            services.AddSingleton<EmployeeManager>();
            services.AddSingleton<FindEmployeeService>();
            services.AddSingleton<EmployeeBankInformationImporter>();

            services.AddSingleton<EmployeeListingVm>();
            services.AddSingleton<Func<EmployeeListingVm>>((s) => () => s.GetRequiredService<EmployeeListingVm>());
            services.AddSingleton<NavigationService<EmployeeListingVm>>();

            return services;
        }
    }
}
