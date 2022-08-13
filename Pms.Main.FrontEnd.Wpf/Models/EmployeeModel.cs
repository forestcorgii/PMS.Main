﻿using Pms.Employees.Domain;
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

        public EmployeeModel(IProvideEmployeeService employeeProvider, IManageEmployeeService employeeManager, IEmployeeFinder employeeFinder)
        {
            _employeeProvider = employeeProvider;
            _employeeManager = employeeManager;
            _employeeFinder = employeeFinder;
        }

        public void SaveEmployee(Employee employee) =>
           _employeeManager.CreateOrEditAndSave(employee);


        public async Task<Employee?> FindEmployeeAsync(string eeId, string site) =>
            await _employeeFinder.GetEmployeeAsync(eeId, site);

        public IEnumerable<Employee> FilterEmployees(string searchString, string payrollCode) =>
            _employeeProvider.FilterEmployees(searchString, payrollCode);

        public IEnumerable<Employee> GetEmployees() =>
            _employeeProvider.GetEmployees();


        //public async Task FindEmployeeAsync(string[] eeIds)
        //{
        //    EmployeeDownloadStarted?.Invoke(this, eeIds.Length);
        //    foreach (string eeId in eeIds)
        //    {
        //        try
        //        {
        //            FindEmployeeService service = new(Adapter);
        //            Employee? employeeFound = await service.GetEmployeeAsync(eeId, Site);
        //            if (employeeFound is null)
        //                employeeFound = new Employee() { EEId = eeId, Active = false };

        //            SaveEmployeeService saveService = new(Context);
        //            saveService.CreateOrEditAndSave(employeeFound);

        //            EmployeeDownloadSucceed?.Invoke(this, eeId);
        //        }
        //        catch (Exception ex)
        //        {
        //            EmployeeDownloadError?.Invoke(this, eeId, ex.Message);
        //        }
        //    }
        // }
    }
}
