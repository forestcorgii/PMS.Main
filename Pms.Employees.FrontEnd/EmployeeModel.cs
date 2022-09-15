﻿using Pms.Employees.Domain;
using Pms.Employees.ServiceLayer;
using Pms.Employees.ServiceLayer.EfCore;
using Pms.Employees.ServiceLayer.Files;
using Pms.Employees.ServiceLayer.HRMS;
using Pms.Employees.ServiceLayer.HRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Employees.FrontEnd.Models
{
    public class EmployeeModel
    {
        EmployeeProvider _employeeProvider;
        EmployeeManager _employeeManager;
        FindEmployeeService _employeeFinder;

        public EmployeeModel(EmployeeProvider employeeProvider, EmployeeManager employeeManager, FindEmployeeService employeeFinder)
        {
            _employeeProvider = employeeProvider;
            _employeeManager = employeeManager;
            _employeeFinder = employeeFinder;
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


        
    }
}