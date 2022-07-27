using Pms.Employees.Domain;
using Pms.Employees.Persistence;
using Pms.Employees.ServiceLayer.Concrete;
using Pms.Employees.ServiceLayer.HRMS.Adapter;
using Pms.Employees.ServiceLayer.HRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Controller
{
    public class EmployeeController
    {
        EmployeeDbContext Context;
        HRMSAdapter Adapter;

        public EmployeeController()
        {
            Context = new EmployeeDbContext();
            Adapter = HRMSAdapterFactory.CreateAdapter(Shared.Configuration);
        }


        public IQueryable<Employee> LoadEmployees()
        {
            var service = new ListEmployeesService(Context);
            return service.GetEmployees();
        }



        public List<string> ListPayrollCodes()
        {
            UtilityService service = new(Context);
            return service.GetEmployeePayrollCodes().ToList();
        }

        public List<string> ListBankCategories(string payrollCodes)
        {
            UtilityService service = new(Context);
            return service.GetEmployeeBankCategory(payrollCodes).ToList();
        }

        public void SaveEmployee(Employee employee)
        {
            var service = new SaveEmployeeService(Context);
            service.CreateOrEditAndSave(employee);
        }
    }
}
