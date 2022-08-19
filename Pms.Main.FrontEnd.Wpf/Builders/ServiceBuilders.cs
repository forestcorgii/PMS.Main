using Microsoft.Extensions.DependencyInjection;
using Pms.Adjustments.Domain.Services;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Employees.Domain.Services;
using Pms.Employees.ServiceLayer;
using Pms.Employees.ServiceLayer.Files;
using Pms.Employees.ServiceLayer.HRMS.Service;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Payrolls.Domain.Services;
using Pms.Payrolls.ServiceLayer.EfCore;
using Pms.Payrolls.ServiceLayer.Files;
using Pms.Payrolls.ServiceLayer.Files.Exports;
using Pms.Timesheets.BizLogic;
using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Builders
{
    static class ServiceBuilders
    {
        public static ServiceCollection AddServices(this ServiceCollection services)
        {
            services.AddSingleton<IProvideEmployeeService, EmployeeProvider>();
            services.AddSingleton<IManageEmployeeService, EmployeeManager>();
            services.AddSingleton<IEmployeeFinder, FindEmployeeService>();
            services.AddSingleton<IImportEmployeeService, ImportEmployeeService>();

            services.AddSingleton<IProvideTimesheetService, ProvideTimesheetService>();
            services.AddSingleton<IDownloadContentProvider, DownloadContentProvider>();
            services.AddSingleton<ITimesheetSaving, SaveTimesheetBizLogic>();

            services.AddSingleton<IManageBillingService, ManageBillingService>();
            services.AddSingleton<IProvideBillingService, ProvideBillingService>();
            services.AddSingleton<IGenerateBillingService, GenerateBillingService>();

            services.AddSingleton<IManagePayrollService, PayrollManager>();
            services.AddSingleton<IProvidePayrollService, PayrollProvider>();
            services.AddSingleton<IImportPayrollService, PayRegisterImport>();
            services.AddSingleton<IExportPayrollService, LBPExport>();


            return services;
        }
    }
}
