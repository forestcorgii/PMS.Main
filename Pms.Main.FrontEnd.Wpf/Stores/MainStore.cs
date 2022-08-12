using Pms.Employees.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.ServiceLayer.EfCore.Queries;
using Pms.Timesheets.ServiceLayer.EfCore.QueryObjects;
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

        private readonly TimesheetStore _timesheetStore;
        private readonly EmployeeStore _employeeStore;


        public MainStore(CutoffTimesheet cutoffTimesheet, TimesheetStore timesheetStore, EmployeeStore employeeStore)
        {
            _timesheetStore = timesheetStore;
            _employeeStore = employeeStore;
            _cutoffTimesheet = cutoffTimesheet;


            Cutoff = new Cutoff();
            CutoffIds = new List<string>();
            PayrollCodes = new List<string>();
            _initializeLoadFiltersLazy = new Lazy<Task>(InitializeLoadFilters);
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
                cutoffIds = _timesheetStore.Timesheets.ExtractCutoffIds();
                payrollCodes = _timesheetStore.Timesheets.ExtractPayrollCodes();
            });

            CutoffIds = cutoffIds;
            PayrollCodes = payrollCodes;
            FiltersReloaded?.Invoke();
        }


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
