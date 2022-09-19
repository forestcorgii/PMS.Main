using Pms.Masterlists.Domain;
using Pms.Masterlists.ServiceLayer;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer.HRMS;
using Pms.Masterlists.ServiceLayer.HRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistModule.FrontEnd.Models
{
    public class Employees
    {
        EmployeeProvider _provider;
        EmployeeManager _manager;
        FindEmployeeService _finder;

        public Employees(EmployeeProvider employeeProvider, EmployeeManager employeeManager, FindEmployeeService employeeFinder)
        {
            _provider = employeeProvider;
            _manager = employeeManager;
            _finder = employeeFinder;
        }

        public bool Exists(string eeId) =>
           _provider.EmployeeExists(eeId);


        public void Save(IPersonalInformation employee) =>
           _manager.Save(employee);

        public void Save(IBankInformation employee) =>
            _manager.Save(employee);

        public void Save(IGovernmentInformation employee) =>
            _manager.Save(employee);

        public void Save(IEEDataInformation employee) =>
            _manager.Save(employee);


        public async Task<Employee> FindEmployeeAsync(string eeId, string site) =>
            await _finder.GetEmployeeAsync(eeId, site);

        public Employee FindEmployee(string eeId) =>
            _provider.FindEmployee(eeId);
         
        public IEnumerable<Employee> GetEmployees() =>
            _provider.GetEmployees();
         

        public IEnumerable<IBankInformation> ImportBankInformation(string payRegisterPath)
        {
            EmployeeBankInformationImporter importer = new();
            return importer.StartImport(payRegisterPath);
        }

        public IEnumerable<IEEDataInformation> ImportEEData(string eeDataPath)
        {
            EmployeeEEDataImporter importer = new();
            return importer.StartImport(eeDataPath);
        }
    }
}
