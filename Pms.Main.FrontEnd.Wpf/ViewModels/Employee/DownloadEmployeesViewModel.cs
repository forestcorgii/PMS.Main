using Pms.Employees.Domain;
using Pms.Employees.Persistence;
using Pms.Employees.ServiceLayer.Concrete;
using Pms.Employees.ServiceLayer.HRMS.Adapter;
using Pms.Employees.ServiceLayer.HRMS.Service;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.ViewModel
{
    public class DownloadEmployeesViewModel
    {
        #region Event Handlers
        public delegate void EmployeeDownloadStartedHandler(object sender, int totalEmployees);
        public delegate void EmployeeDownloadSucceedHandler(object sender, string eeId);
        public delegate void EmployeeDownloadErrorHandler(object sender, string eeId, string errorMessage);
        #endregion

        #region Event
        public event EmployeeDownloadStartedHandler? EmployeeDownloadStarted;
        public event EmployeeDownloadSucceedHandler? EmployeeDownloadSucceed;
        public event EmployeeDownloadErrorHandler? EmployeeDownloadError;
        #endregion

        EmployeeDbContext Context;
        HRMSAdapter Adapter;


        public string PayrollCode { get; private set; }
        private string Site { get; }

        public DownloadEmployeesViewModel(string payrollCode)
        {
            //Context = new EmployeeDbContext();
            Adapter = HRMSAdapterFactory.CreateAdapter(Shared.Configuration);

            PayrollCode = payrollCode;

            if (PayrollCode[0] == 'L') Site = "LEYTE";
            else Site = "MANILA";
        }

        public async Task<Employee?> FindEmployeeAsync(string eeId)
        {
            FindEmployeeService service = new(Adapter);
            Employee? employeeFound = await service.GetEmployeeAsync(eeId, Site);
            if (employeeFound is not null)
                return employeeFound;

            return default;
        }

        public async Task FindEmployeeAsync(string[] eeIds)
        {
            //EmployeeDownloadStarted?.Invoke(this, eeIds.Length);
            //foreach (string eeId in eeIds)
            //{
            //    try
            //    {
            //        FindEmployeeService service = new(Adapter);
            //        Employee? employeeFound = await service.GetEmployeeAsync(eeId, Site);
            //        if (employeeFound is null)
            //            employeeFound = new Employee() { EEId = eeId, Active = false };

            //        SaveEmployeeService saveService = new(Context);
            //        saveService.CreateOrEditAndSave(employeeFound);

            //        EmployeeDownloadSucceed?.Invoke(this, eeId);
            //    }
            //    catch (Exception ex)
            //    {
            //        EmployeeDownloadError?.Invoke(this, eeId, ex.Message);
            //    }
            //}
        }


    }
}
