using Payroll.Employees.Domain;
using Payroll.Employees.Persistence;
using Payroll.Employees.ServiceLayer.HRMS.Adapter;
using Payroll.Employees.ServiceLayer.HRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class EmployeeDownloadController
    {
        EmployeeDbContext Context;
        HRMSAdapter Adapter;

        public EmployeeDownloadController()
        {
            Context = new EmployeeDbContext();
            Adapter = HRMSAdapterFactory.CreateAdapter(Shared.Configuration);
        }

        public async Task<Employee?> FindEmployeeAsync(string eeId)
        {
            FindEmployeeService service = new(Adapter);
            Employee? employeeFound = await service.GetEmployeeAsync(eeId);
            if (employeeFound is not null)
                return employeeFound;

            return default;
        }


    }
}
