using Pms.Employees.Domain;
using Pms.Employees.Domain.Services;
using Pms.Employees.ServiceLayer.EfCore;
using Pms.Employees.ServiceLayer.HRMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Models
{
    public class EmployeeModel
    {
        IProvideEmployeeService _employeeProvider;
        IManageEmployeeService _employeeManager;
        IEmployeeFinder _employeeFinder;
        IImportEmployeeService _employeeImporter;

        public EmployeeModel(IProvideEmployeeService employeeProvider, IManageEmployeeService employeeManager, IEmployeeFinder employeeFinder, IImportEmployeeService employeeImporter)
        {
            _employeeProvider = employeeProvider;
            _employeeManager = employeeManager;
            _employeeFinder = employeeFinder;
            _employeeImporter = employeeImporter;
        }

        public bool Exists(string eeId) =>
           _employeeProvider.EmployeeExists(eeId);


        public void Save(IPersonalInformation employee) =>
           _employeeManager.Save(employee);

        public void Save(IBankInformation employee) =>
            _employeeManager.Save(employee);

        public void Save(IGovernmentInformation employee) =>
            _employeeManager.Save(employee);


        public async Task<Employee> FindEmployeeAsync(string eeId, string site) =>
            await _employeeFinder.GetEmployeeAsync(eeId, site);

        public Employee FindEmployee(string eeId) =>
            _employeeProvider.FindEmployee(eeId);

        public IEnumerable<Employee> FilterEmployees(string searchString, string payrollCode) =>
            _employeeProvider.FilterEmployees(searchString, payrollCode);

        public IEnumerable<Employee> GetEmployees() =>
            _employeeProvider.GetEmployees();


        public IEnumerable<IBankInformation> Import(string payRegisterPath) =>
            _employeeImporter.StartImport(payRegisterPath);
    }
}
