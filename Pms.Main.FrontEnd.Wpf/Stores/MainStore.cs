﻿using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class MainStore
    {
        #region MAIN
        public Cutoff Cutoff { get; private set; }
        public string PayrollCode { get; private set; } = "";
        public string Site
        {
            get
            {
                if ((PayrollCode != "" && PayrollCode[0] == 'L') || PayrollCode == "P4A")
                    return "LEYTE";
                else
                    return "MANILA";
            }
        }

        public List<string> CutoffIds { get; private set; }
        public List<string> PayrollCodes { get; private set; }
        public event Action? FiltersReloaded;
        private Lazy<Task> _initializeLoadFiltersLazy;
        #endregion

        private CutoffTimesheet _cutoffTimesheet;
        //#region TIMESHEET
        //private readonly List<Timesheet> _timesheets;
        //public IEnumerable<Timesheet> Timesheets => _timesheets;
        //private Lazy<Task> _initializeLoadTimesheetsLazy;
        //public event Action? TimesheetsReloaded;
        //#endregion

        //#region EMPLOYEE
        //private EmployeeModel _employeeModel;
        //private readonly List<Employee> _employees;
        //public IEnumerable<Employee> Employees => _employees;
        //private Lazy<Task> _initializeLoadEmployeesLazy;
        //public event Action? EmployeesReloaded;
        //#endregion

        private readonly TimesheetStore _timesheetStore;
        private readonly EmployeeStore _employeeStore;


        public MainStore(CutoffTimesheet cutoffTimesheet,TimesheetStore timesheetStore, EmployeeStore employeeStore)
        {
            _timesheetStore = timesheetStore;
            _employeeStore = employeeStore;
            _cutoffTimesheet = cutoffTimesheet;
            // MAIN
            Cutoff = new Cutoff();
            CutoffIds = new List<string>();
            PayrollCodes = new List<string>();
            _initializeLoadFiltersLazy = new Lazy<Task>(InitializeLoadFilters);

            //// TIMESHEET
            //_timesheets = new List<Timesheet>();
            //_initializeLoadTimesheetsLazy = new Lazy<Task>(InitializeLoadTimesheets);

            //// EMPLOYEE
            //_employees = new List<Employee>();
            //_employeeModel = employeeModel;
            //_initializeLoadEmployeesLazy = new Lazy<Task>(InitializeLoadEmployees);
        }

        public async Task LoadFilters()
        {
            try
            {
                await _initializeLoadFiltersLazy.Value;
            }
            catch (Exception)
            {
                _initializeLoadFiltersLazy = new Lazy<Task>(InitializeLoadFilters);
                throw;
            }
        }

        public async Task InitializeLoadFilters()
        {
            List<string> cutoffIds = new();
            List<string> payrollCodes = new();
            await Task.Run(() =>
            {
                cutoffIds = _cutoffTimesheet.ListTimesheetCutoffIds();
                payrollCodes = _cutoffTimesheet.ListTimesheetPayrollCodes();
            });

            CutoffIds = cutoffIds;
            PayrollCodes = payrollCodes;
            FiltersReloaded?.Invoke();
        }


        //public async Task LoadTimesheets()
        //{
        //    try
        //    {
        //        await _initializeLoadTimesheetsLazy.Value;
        //    }
        //    catch (Exception)
        //    {
        //        _initializeLoadTimesheetsLazy = new Lazy<Task>(InitializeLoadTimesheets);
        //        throw;
        //    }
        //}

        //public async Task ReloadTimesheets()
        //{
        //    _initializeLoadTimesheetsLazy = new Lazy<Task>(InitializeLoadTimesheets);
        //    await _initializeLoadTimesheetsLazy.Value;
        //}

        //private async Task InitializeLoadTimesheets()
        //{
        //    if (Cutoff is not null)
        //    {
        //        IEnumerable<Timesheet> timesheets = new List<Timesheet>();
        //        await Task.Run(() =>
        //        {
        //            timesheets = _cutoffTimesheet.GetTimesheets(Cutoff.CutoffId, PayrollCode);
        //        });

        //        _timesheets.Clear();
        //        _timesheets.AddRange(timesheets);
        //        TimesheetsReloaded?.Invoke();
        //    }
        //}



        //public async Task LoadEmployees()
        //{
        //    try
        //    {
        //        await _initializeLoadEmployeesLazy.Value;
        //    }
        //    catch (Exception)
        //    {
        //        _initializeLoadEmployeesLazy = new Lazy<Task>(InitializeLoadEmployees);
        //        throw;
        //    }
        //}

        //public async Task ReloadEmployees()
        //{
        //    _initializeLoadEmployeesLazy = new Lazy<Task>(InitializeLoadEmployees);
        //    await _initializeLoadEmployeesLazy.Value;
        //}

        //private async Task InitializeLoadEmployees()
        //{
        //    IEnumerable<Employee> employees = new List<Employee>();
        //    await Task.Run(() =>
        //    {
        //        employees = _employeeModel.FilterEmployees("", PayrollCode);
        //    });

        //    _employees.Clear();
        //    _employees.AddRange(employees);
        //    EmployeesReloaded?.Invoke();
        //}



        public void SetCutoff(Cutoff cutoff)
        {
            Cutoff = cutoff;
            _timesheetStore.SetCutoffId(cutoff.CutoffId);
        }

        public void SetPayrollCode(string payrollCode)
        {
            PayrollCode = payrollCode;
            Cutoff.SetSite(Site);
            _timesheetStore.SetPayrollCode(payrollCode);
            _employeeStore.SetPayrollCode(payrollCode);
        }

    }
}