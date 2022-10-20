using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Entities.Employees;
using Pms.Masterlists.ServiceLayer;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Files;
using Pms.Masterlists.ServiceLayer.HRMS;
using Pms.Masterlists.ServiceLayer.HRMS.Services;
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
        HrmsEmployeeProvider _finder;

        public Employees(EmployeeProvider employeeProvider, EmployeeManager employeeManager, HrmsEmployeeProvider employeeFinder)
        {
            _provider = employeeProvider;
            _manager = employeeManager;
            _finder = employeeFinder;
        }

        public bool Exists(string eeId) =>
           _provider.EmployeeExists(eeId);


        public void Save(Employee employee) =>
            _manager.Save(employee);

        public void Save(IActive employee) =>
            _manager.Save(employee);

        public void Save(IMasterFileInformation employee) =>
            _manager.Save(employee);

        public void Save(IHRMSInformation employee) =>
            _manager.Save(employee);

        public void Save(IBankInformation employee) =>
            _manager.Save(employee);

        public void Save(IGovernmentInformation employee) =>
            _manager.Save(employee);

        public void Save(IEEDataInformation employee) =>
            _manager.Save(employee);


        public async Task<Employee> SyncOneAsync(string eeId, string site) =>
            await _finder.GetEmployeeAsync(eeId, site);

        public async Task<IEnumerable<Employee>> SyncNewlyHiredAsync(DateTime fromDate, string site)
        {
            IEnumerable<Employee>? result = await _finder.GetNewlyHiredEmployeesAsync(fromDate, site);
            if (result is not null)
                return result;

            return Enumerable.Empty<Employee>();
        }

        public async Task<IEnumerable<Employee>> SyncResignedAsync(DateTime fromDate, string site)
        {
            IEnumerable<Employee>? result = await _finder.GetResignedEmployeesAsync(fromDate, site);
            if (result is not null)
                return result;

            return Enumerable.Empty<Employee>();
        }


        public Employee FindEmployee(string eeId) =>
            _provider.FindEmployee(eeId);

        public IEnumerable<Employee> GetEmployees() =>
            _provider.GetEmployees();


        public IEnumerable<IBankInformation> ImportBankInformation(string payRegisterPath)
        {
            EmployeeBankInformationImporter importer = new();
            return importer.StartImport(payRegisterPath);
        }

        public IEnumerable<IMasterFileInformation> ImportMasterFile(string payRegisterPath)
        {
            MasterFileImporter importer = new();
            return importer.StartImport(payRegisterPath);
        }

        public IEnumerable<IEEDataInformation> ImportEEData(string eeDataPath)
        {
            EmployeeEEDataImporter importer = new();
            return importer.StartImport(eeDataPath);
        }


        public void ExportMasterlist(IEnumerable<Employee> employees, PayrollCode payrollCode, string remarks = "")
        {
            MasterlistExporter exporter = new();
            exporter.StartExport(employees, payrollCode, remarks);
        }

        public void ReportExceptions(IEnumerable<Exception> exceptions, PayrollCode payrollCode, string suffix)
        {
            if (exceptions.Count() > 0)
            {
                InvalidValueReporter exporter = new();
                exporter.StartReport(exceptions, payrollCode, suffix);

            }
        }
    }
}
