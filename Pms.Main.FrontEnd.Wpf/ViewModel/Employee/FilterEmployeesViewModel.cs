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
    public class FilterEmployeesViewModel
    {
        EmployeeDbContext Context;


        public string PayrollCode { get; private set; }

        public FilterEmployeesViewModel(string payrollCode)
        {
            Context = new EmployeeDbContext();

            PayrollCode = payrollCode;
        }

        public List<Employee> ListEmployeesByPayrollCode(string searchString)
        {
            ListEmployeesService service = new(Context);
            return service.FilterEmployees(searchString, PayrollCode).ToList();
        }

        public List<Employee> ListEmployeesByPayrollCode()
        {
            ListEmployeesService service = new(Context);
            return service.FilterEmployees("", PayrollCode).ToList();
        }

        public List<Employee> ListAllEmployees()
        {
            ListEmployeesService service = new(Context);
            return service.GetEmployees().ToList();
        }

        public List<string> ListPayrollCodes()
        {
            ListEmployeesService service = new(Context);
            return service.ListEmployeePayrollCodes().ToList();
        }

        public List<string> ListBankCategories(string payrollCodes)
        {
            ListEmployeesService service = new(Context);
            return service.ListEmployeeBankCategory(payrollCodes).ToList();
        }
    }
}
